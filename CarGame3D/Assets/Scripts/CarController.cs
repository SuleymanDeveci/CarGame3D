using System;
using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    [SerializeField] public WheelColliders _wheelColliders;     // aracin tekerlek colliderlerinin bulundugu sinifi newliyoruz
    [SerializeField] private WhellMeshes _wheelMeshes;          // aracin tekerleklerinin MeshRenderer'lerinin bulundugu sinifi newliyoruz
    [SerializeField] private WheelParticles _wheelParticles;    // her tekerin cikaracagi duman particle larini ayri ayri tutan sinifi newliyoruz
    [SerializeField] private GameObject _smokePrefab;           // icinde tekerlek dumani icin olusturulan ParticleSystem'in yer aldigi bir GameObject
    
    private GearController _gearController;                     // Vites bilgilerini aldigimiz script
    private Rigidbody _rb;
    private WheelFrictionCurve _handBrakeFriction;              // elfreni cekildigi zaman arka lastiklere uygulanacak olan surtunme katsayisi degerleri
    private WheelFrictionCurve _defaultFriction;                // arka tekerlerin varsayilan surtunme katsayisi
    

    [SerializeField] private float _maxSpeed;              // aracin maksimum hiz bilgisini tutan degisken
    [SerializeField] private float _motorPower;            // aracin gucunu tutan degisken
    [SerializeField] private float _brakePower;            // aracin fren gucunu tutan degisken
    [SerializeField] private float _handBrakePower;        // aracin el fren gucunu tutan degisken
    [SerializeField] private float _handBrakeStiffness;    // el freni cekildigi zaman arka tekerlerin surtunme katsayisi
    [SerializeField] private float _defaultStiffness;      // normal durumda arka tekerlerin surtunme katsayisi

    private float _gasInput;       // gaza ne kadar bastigimizin bilgisini tutacak olan degisken
    private float _brakeInput;     // frene ne kadar bastigimizin bilgisini tutacak olan degisken
    private float _handBrakeInput; // el frenine ne kadar bastigimizin bilgisini tutacak olan degisken
    private float _steeringInput;  // direksiyonu ne kadar cevirdigimizin bilgisini tutacak olan degisken
    public float _slipAngle;      // aracin kac derece kaydigi bilgisini tutan degisken
    private float _steeringAngle;  // aracin dreksiyonunun kac derece dondugu bilgisini tutan degisken
    [SerializeField] private float _speed;          // aracin hiz bilgisini tutan degisken
    public float SpeedSmooth { get; private set; }   // _speed degiskeni'nin ani degisimlerden etkilenmeyen hali (get; private set; diger siniflardan erismek icin)

    public AnimationCurve steeringCurve; //arac hizli giderken daha az, yavas giderken daha cok direksiyon donus acisi olsun diye kullandigim degisken (100 birim hizla giderken 15 derece -
                                         // (1 birim hizla giderken 40 derece donus acisi olacak sekilde ayarlandi)

    private void Awake()
    {
        _gearController = GetComponent<GearController>();
        _rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        
        InstantiateSmoke();
        _gearController.AutoGearSlider.onValueChanged.AddListener(delegate { OnAutoGearValueChange(); }); // sliderin degeri her degistiginde, yani her vites degistiginde "OnAutoGearValueChange" fonk. tetiklenir

        _handBrakeFriction = _wheelColliders.RRWheel.sidewaysFriction;    // gerekli degerleri atamasini yapiyoruz
        _defaultFriction = _wheelColliders.RRWheel.sidewaysFriction;      // gerekli degerleri atamasini yapiyoruz, eger bunu yapmazsak icerisindeki tum degerler 0 olur. bir asagi satirda stiffness
                                                                          // degeri icin bir atama yapiliyor ama diger degerler oldugu gibi kalmasi icin bu kodu yazmamiz gerekiyor

        _handBrakeFriction.stiffness = _handBrakeStiffness;               // el freni cekili iken arka tekerlerin alacagi surtunme katsayisini tanimliyoruz 
        _defaultFriction.stiffness = _defaultStiffness;                   // normalde arka tekerlerin alacagi surtunme katsayisini tanimliyoruz
    }

    private void Update()
    {
        _speed = _wheelColliders.FRWheel.rpm * (_wheelColliders.FRWheel.radius * (2f * Mathf.PI)) / 10f; // _speed degiskenini aracin on tekerinin kac kez dondugunden aldigimiz veri ile hesapliyoruz
        SpeedSmooth = Mathf.Lerp(SpeedSmooth, _speed, Time.deltaTime); // Lerp komutu sayesinde _speed degiskeni'ni biraz geriden takip ediyor ve _speed degiskeni'nin ani degisimlerinden etkilenmeyerek 
                                                                         // _speedSmooth degerini yavas yavas arttiriyor veya azaltiyoruz (aracin sesi icin kullaniyoruz)

        CheckInput();
        ApplySteering();
        ApplyWheelPosition();
        CheckParticles();
    }

    private void OnAutoGearValueChange() // her vites degistiginde _gasInput degerini "0" a esitliyoruz
    {
        _gasInput = 0;
        ApplyMotorZeroTorque(); // tekerlere uygulanan torku 0 yapiyoruz 
    }
    private void InstantiateSmoke()  // Her tekerlek icin, icinde ParticleSystem bulunan bir GameObject olusturulup transformlari ayarlanip _wheelParticles nesnesinin içerisi dolduruluyor
    {
        _wheelParticles.FRWheelParticle = Instantiate(_smokePrefab, _wheelColliders.FRWheel.transform.position - Vector3.up * _wheelColliders.FRWheel.radius, Quaternion.identity,
            _wheelColliders.FRWheel.transform).GetComponent<ParticleSystem>(); // Sag On teker icin, icerisinde ParticleSystem bulunan bir GameObject olusturulup, konumu tekerin en alti
                                                                               // olacak sekilde ayarlanip, Particlelerin tekerlek ile birlikte hareket etmesi için _wheelColliders.FRWheel.transform'u kendisine parrent olarak ataniyor,
                                                                               // ve bu GameObject icerisindeki ParticleSystem Componenti alinip, _wheelParticles.FRWheelParticle degiskeni icerisine ataniyor.
                                                                               // bu sayede _wheelParticles.FRWheelParticle ParticleSystemi istenildigi zaman istenildigi yerden yonetilebilir

        _wheelParticles.FLWheelParticle = Instantiate(_smokePrefab, _wheelColliders.FLWheel.transform.position - Vector3.up * _wheelColliders.FRWheel.radius, Quaternion.identity,
            _wheelColliders.FLWheel.transform).GetComponent<ParticleSystem>();

        _wheelParticles.RRWheelParticle = Instantiate(_smokePrefab, _wheelColliders.RRWheel.transform.position - Vector3.up * _wheelColliders.FRWheel.radius, Quaternion.identity,
            _wheelColliders.RRWheel.transform).GetComponent<ParticleSystem>();

        _wheelParticles.RLWheelParticle = Instantiate(_smokePrefab, _wheelColliders.RLWheel.transform.position - Vector3.up * _wheelColliders.FRWheel.radius, Quaternion.identity,
            _wheelColliders.RLWheel.transform).GetComponent<ParticleSystem>();
    }

    private void CheckInput() // oyuncudan alinan input verileri gerekli degiskenlere ataniyor
    {
        if (Input.GetKeyDown(KeyCode.R))   // eger R tusuna basildi ise    ( Arac'in ters dondugu durumlarda kullanmak icin)
        {
            _rb.velocity = Vector3.zero;    // aracin hizini 0'a eþitle
            _rb.angularVelocity = Vector3.zero;  // aracin acisal hizini 0'a esitle
            gameObject.transform.position = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);   // aracin pozisyonunu yerden 1f yukseklige ayarla
            gameObject.transform.rotation = Quaternion.Euler(0,transform.rotation.eulerAngles.y,0);    //    aracin y eksenindeki donusu haric diger eksenlerdeki donusunu sifirla
            
        }
        if (_gearController.AutoGearSlider.value == 1)  // eger vites D konumunda ise true
        {
            if (Input.GetKey(KeyCode.W)) // "W" ye basili tutuldugu süre boyunca true
            {
                if (_gasInput < 1) // direk tusa basildigi anda _gasInput = 1 diyerek aracin cok sert kalkmasini engellemek icin, _gasInput'u yavas yavas artiriyoruz
                {
                    _gasInput += 2f * Time.deltaTime;
                }
                else // gas input'un 1 degerini gecmeyip sabit kalmasini sagliyoruz.
                {
                    _gasInput = 1;
                }
                ApplyMotor();
            }
            else if (Input.GetKeyUp(KeyCode.W)) // elimizi "W" tusundan ceker cekmez _gasInputu 0 a esitliyoruz
            {
                _gasInput = 0;
                ApplyMotorZeroTorque();
            }
        }
        else if (_gearController.AutoGearSlider.value == 3) // eger vites R konumunda ise true
        {
            if (Input.GetKey(KeyCode.W)) // "W" ye basili tutuldugu süre boyunca true
            {
                if (_gasInput > -1) // direk tusa basildigi anda _gasInput = -1 diyerek aracin cok sert geri geri gitmesini engellemek icin, _gasInput'u yavas yavas azaltiyoruz
                {
                    _gasInput -= 2f * Time.deltaTime;
                }
                else // gas input'un -1 degerinin altina inmeyip sabit kalmasini sagliyoruz.
                {
                    _gasInput = -1;
                }
                ApplyMotor();
            }
            else if (Input.GetKeyUp(KeyCode.W)) // elimizi "W" tusundan ceker cekmez _gasInputu 0 a esitliyoruz
            {
                _gasInput = 0;
                ApplyMotorZeroTorque();
            }
        }

        if (Input.GetKey(KeyCode.S)) // "S" ye basili tutuldugu süre boyunca true
        {
            if (_brakeInput < 1) // direk tusa basildigi anda _brakeInput = 1 diyerek aracin cok sert durmasini engellemek icin, _brakeInput'u yavas yavas artiriyoruz
            {
                _brakeInput += 1f * Time.deltaTime;
            }
            else // _brakeInput'un 1 degerini gecmeyip sabit kalmasini sagliyoruz.
            {
                _brakeInput = 1;
            }
            ApplyNormalBrake();
        }

        if (Input.GetKeyUp(KeyCode.S)) // elimizi "S" tusundan ceker cekmez _brakeInput'u 0 a esitliyoruz
        {
            _brakeInput = 0;
            ApplyNormalBrake();
        }

        if (Input.GetKey(KeyCode.Space)) // "Space" ye basili tutuldugu süre boyunca true
        {
            _handBrakeInput = 1;  // yavas yavas artirmak yerine "Space" basildigi anda _handBrakeInput'u 1 yapiyoruz cunku el freni cekildigi anda arka tekerler kilitlensin istiyoruz
            ApplyHandBrake();
            ReduceFriction();
        }
        else if (Input.GetKeyUp(KeyCode.Space)) // elimizi "Space" tusundan ceker cekmez _handBrakeInput'u 0 a esitliyoruz
        {
            _handBrakeInput = 0;
            ApplyHandBrake();
            IncreaseFriction();
        }

        _steeringInput = Input.GetAxis("Horizontal"); // D tusuna basildigi zaman _steeringInput degiskeni 1, A tusuna basildigi zaman -1 degerine gider
        _slipAngle = Vector3.Angle(transform.forward, _rb.velocity - transform.forward); // aracin kac derece kaydigi bilgisi ataniyor
    }

    private void ApplyNormalBrake() // oyuncu alýnan fren bilgisi tekerleklere uygulaniyor
    {
        _wheelColliders.FRWheel.brakeTorque = _brakeInput * _brakePower * 0.7f; // fren gucunun yuzde 70'i on tekerlere uygulaniyor
        _wheelColliders.FLWheel.brakeTorque = _brakeInput * _brakePower * 0.7f;

        _wheelColliders.RRWheel.brakeTorque = _brakeInput * _brakePower * 0.3f; // fren gucunun yuzde 30'u arka tekerlere uygulaniyor
        _wheelColliders.RLWheel.brakeTorque = _brakeInput * _brakePower * 0.3f;
    }

    private void ReduceFriction() // el freni tusuna basildigi zaman sadece arka iki tekerin surtunme katsayilari dusuruluyor
    {
        _wheelColliders.RRWheel.sidewaysFriction = _handBrakeFriction;  // sag arka tekerin surtunme katsayisi dusuruluyor
        _wheelColliders.RLWheel.sidewaysFriction = _handBrakeFriction;  // sol arka tekerin surtunme katsayisi dusuruluyor
    }
    private void IncreaseFriction() // el freni tusu birakildigi zaman arka iki tekerin surtunme katsayilari normal degerlerine donduruluyor
    {
        _wheelColliders.RRWheel.sidewaysFriction = _defaultFriction;  // sag arka tekerin surtunme katsayisi normale cikariliyor
        _wheelColliders.RLWheel.sidewaysFriction = _defaultFriction;  // sol arka tekerin surtunme katsayisi normale cikariliyor
    }
    private void ApplyHandBrake() // el freni tusuna basildigi zaman sadece arka iki tekere fren uygulaniyor
    {
        _wheelColliders.RRWheel.brakeTorque = _handBrakePower * _handBrakeInput;  // sag arka tekere fren uygulaniyor
        _wheelColliders.RLWheel.brakeTorque = _handBrakePower * _handBrakeInput;  // sol arka tekere fren uygulaniyor
    }

    private void ApplyMotor() // oyuncudan aldigimiz _gasInput verisine gore motora (arka tekerlere) guc uyguluyoruz
    {
        if (_speed < _maxSpeed) // eger hiz maksimum hizdan daha kucukse
        {
            _wheelColliders.RRWheel.motorTorque = _motorPower * _gasInput; // aracin arka tekerlerine guc uygula
            _wheelColliders.RLWheel.motorTorque = _motorPower * _gasInput;

        }
        else     // degilse
        {
            _wheelColliders.RRWheel.motorTorque = 0; // aracin arka tekerlerine 0 guc uygula
            _wheelColliders.RLWheel.motorTorque = 0;
            // aracin tekerine 0 guc uygula demek yerine else kismini hic yazmayip zaten eger _speed < _maxSpeed false dondururse araca guc uygulanmayacak diye dusunebilirisin ama
            // _wheelColliders.RLWheel.motorTorque methodu kendisine uygulanan en son gucu aksi soylenmedikce surekli uygulamaya devam ediyor
            // dolayisi ile tekerlere guc uygulamayi kesmek icin 1 kez olsa bile _wheelColliders.RLWheel.motorTorque = 0 demek zorundayiz
        }

    }
    private void ApplyMotorZeroTorque() // tekerlere daha fazla guc uygulamak istemedigimiz zaman 1 kez calistirmamiz gereken kod, cunku "_wheelColliders.RRWheel.motorTorque" bu kod
                                        // sen anlik olarak cagirmasanda, en son aldigi degeri surekli tekerlere uyguluyor
    {
        _wheelColliders.RRWheel.motorTorque = 0; // sag arka tekere 0 guc uygula
        _wheelColliders.RLWheel.motorTorque = 0; // sol arka tekere 0 guc uygula
    }

    private void ApplySteering() // oyuncudan alinan direksiyon bilgisi once hiza gore hesaplanip sonra on tekerlere uygulaniyor
    {
        _steeringAngle = _steeringInput * steeringCurve.Evaluate(SpeedSmooth); // direksiyon acisini hiz degiskenine gore artirip azaltiyor (maksimum 40 derece - minimum 15 derece Inspectorden ayarlandi)

        
        if(_gearController.AutoGearSlider.value == 1) // eger vites D konumunda ise
        {
            _steeringAngle += Vector3.SignedAngle(transform.forward, _rb.velocity + transform.forward, Vector3.up); // signedAngle tipki Angle methodu gibi iki vektor arasindaki aci farkini verir ama negatifler de dahil
                   // burada direksiyona counter acý veriyoruz. bunuda aracin gittigi yon ile baktigi yon arasindaki aci farkini hesaplayarak ve bu farki on tekerleklere ters yonde uygulayarak yapiyoruz.
                   // Ayrica bunu sadece aracnin vitesi D konumunda ise yapiyoruz cunku arac geri geri giderken, gittigi yon ile baktigi yon bambaska ve geri geri giderken boyle bir sisteme ihtiyac duyulmuyor

            _steeringAngle = Mathf.Clamp(_steeringAngle, -40f, 40f); // direksiyon acisini -40 ile 40 arasina indiriyoruz
        }
        _wheelColliders.FRWheel.steerAngle = _steeringAngle; // elde ettigimiz degeri on tekerlere direksiyon acisi uyguluyoruz
        _wheelColliders.FLWheel.steerAngle = _steeringAngle;
    }

    private void ApplyWheelPosition() // UpdateWheel Fonksiyon'unu her teker icin ayri ayri cagiriyor
    {
        UpdateWheel(_wheelColliders.FRWheel, _wheelMeshes.FRWheelMesh);
        UpdateWheel(_wheelColliders.FLWheel, _wheelMeshes.FLWheelMesh);
        UpdateWheel(_wheelColliders.RRWheel, _wheelMeshes.RRWheelMesh);
        UpdateWheel(_wheelColliders.RLWheel, _wheelMeshes.RLWheelMesh);
    }

    private void CheckParticles() // Particlelerin olusma zamanini kontrol ediyoruz
    {
        WheelHit[] wheelHits = new WheelHit[4];                   // tekerlerin yere temas edip etmedigi bilgisini tutan degiskenn (Unity Ozelligi)
        _wheelColliders.FRWheel.GetGroundHit(out wheelHits[0]);    // GetGroundHit metodu, WheelHit türünde bir degisken donduruyor, bu degiskenide wheelHit[] arrayinin icerisine atiyoruz
                                                                   // ***ONEMLI BILGI*** out keywordunu kullanmamin sebebi wheelHits[0] degiskeninin methodun icerisine bilgi tasiyan bir parametre degilde,
                                                                   // metodun disarisina bilgi cikaran bir parametre oldugunu belirtmek
        _wheelColliders.FLWheel.GetGroundHit(out wheelHits[1]);

        _wheelColliders.RRWheel.GetGroundHit(out wheelHits[2]);
        _wheelColliders.RLWheel.GetGroundHit(out wheelHits[3]);

        float slipAllowance = 0.3f;   // tekerlerin particle sacmadan once kaymasina izin verilen miktar

        // asagida yazdigim kod icerisinde foreach kullanamadim cunku her tekerin particleSistemini _wheelParticles.FRWheelParticle seklinde isimlendirdim.
        // eger particle sistemlerini 4 indexli bir array icerisinde tutsaydim foreach kullanabilirdim ve asagidaki gibi kod kalabaligi olmazdi
        // ama particle sistemlerini array icerisinde tanimlasaydim bu sefer kacinci indexin hangi tekerin particle sistemini temsil ettigini anlamak zor olurdu
        // yani array ile yapsaydým kodun okunabilirligi bence daha kotu olurdu 
        if ((Mathf.Abs(wheelHits[0].sidewaysSlip) + Mathf.Abs(wheelHits[0].forwardSlip) > slipAllowance)) // eger sag on tekerin yanal kayma degeri ile dikey kayma degerleri toplami izin verilen kayma degerinden buyukse 
        {
            _wheelParticles.FRWheelParticle.Play(); // sag on teker particle yaymaya baslasin
        }
        else // degilse
        {
            _wheelParticles.FRWheelParticle.Stop(); // sag on teker particle yaymayi durdursun
        }

        if ((Mathf.Abs(wheelHits[1].sidewaysSlip) + Mathf.Abs(wheelHits[1].forwardSlip) > slipAllowance)) // eger sol on tekerin yanal kayma degeri ile dikey kayma degerleri toplami izin verilen kayma degerinden buyukse 
        {
            _wheelParticles.FLWheelParticle.Play(); // sol on teker particle yaymaya baslasin
        }
        else // degilse
        {
            _wheelParticles.FLWheelParticle.Stop(); // sol on teker particle yaymayi durdursun
        }

        if ((Mathf.Abs(wheelHits[2].sidewaysSlip) + Mathf.Abs(wheelHits[2].forwardSlip) > slipAllowance)) // eger sag arka tekerin yanal kayma degeri ile dikey kayma degerleri toplami izin verilen kayma degerinden buyukse 
        {
            _wheelParticles.RRWheelParticle.Play(); // sag arka teker particle yaymaya baslasin
        }
        else // degilse
        {
            _wheelParticles.RRWheelParticle.Stop(); // sag arka teker particle yaymayi durdursun
        }

        if ((Mathf.Abs(wheelHits[3].sidewaysSlip) + Mathf.Abs(wheelHits[3].forwardSlip) > slipAllowance)) // eger sol arka tekerin yanal kayma degeri ile dikey kayma degerleri toplami izin verilen kayma degerinden buyukse 
        {
            _wheelParticles.RLWheelParticle.Play(); // sol arka teker particle yaymaya baslasin
        }
        else // degilse
        {
            _wheelParticles.RLWheelParticle.Stop(); // sol arka teker particle yaymayi durdursun
        }
    }

    // Asagidaki kodu anlamak icin once sunu bilmeliyiz: Her tekerlek icin 2 Adet GameObject tanimladim, bir tanesinin icerisinde sadece WheelCollider var, digerinin icerisinde ise MeshRenderer var
    private void UpdateWheel(WheelCollider coll, MeshRenderer wheelMesh) // icerisinde WheelCollider bulunan GameObject'in Position ve Quaternion bilgisini alip,
                                                                         // icerisinde MeshRenderer bulunan GameObject'in icerisine atiyorum, boylelikle tekerin dondugu goruntusu olusuyor
    {
        Quaternion quat; // colliderden alinan Quaternion bilgisini tutmak icin tanimlandi
        Vector3 position; // colliderden alinan Position bilgisini tutmak icin tanimlandi
        coll.GetWorldPose(out position, out quat); // WheelColliderden dunya uzerindeki Position ve Quaternion bilgisi aliniyor
        // ***ONEMLI BILGI*** "out" keywordu methoda gonderilen bir parametre gibi gozukur ancak amacý metodun icerisinde kullanýlmak uzere bir bilgi goturmek degil, metodun icerisinden disariya(out) bilgi getirmektir
        // GetWorldPose metodu'nun tanimlandigi yerde de parametrelerinin basinda out keywordu bulunuyor, yani metod oluþturulurken bastan bunun icin olusturulmus
        wheelMesh.transform.position = position;  // Wheelcollider'den alinan position bilgisi WheelMesh'e aktarilarak tekerin gitme goruntusunu goruyoruz (aslinda bu olmasa da olur - 
                                                  // cunku tekerlek aracin body'sine sabit ve onunla ilerliyor)
        wheelMesh.transform.rotation = quat;    // Wheelcollider'den alinan Quaternion bilgisi WheelMesh'e aktarilarak tekerin donme goruntusunu goruyoruz
    }

    public float GetSpeedRatio() // gasinput degiskeni'nin 0.5 ile 1 arasina clamp yapilmis hali ile arac hizi'nin normalize edilmis halinin carpimini dondurur
    {
        float gas = Mathf.Clamp(_gasInput, 0.5f, 1f); // gasinput degiskenini 0.5 ile 1 arasina daraltir
        return (SpeedSmooth / _maxSpeed) * gas; // normalize edilmis _speed degiskeni ile gas degiskeni carpilarak return edilir.
                                                 // _speedSmooth: _speed degiskenini biraz geriden takip eden, _speed degiskeni'nin aksine ani degisikler yerine daha yumsak gecis degerleri alan degisken 
    }
}
