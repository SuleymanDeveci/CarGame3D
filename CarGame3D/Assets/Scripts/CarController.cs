using UnityEngine;
using UnityEngine.UI;

public class CarController : MonoBehaviour
{
    private Rigidbody rb;
    public WheelColliders wheelColliders;  // aracin tekerlek colliderlerinin bulundugu sinifi newliyoruz
    public WhellMeshes wheelMeshes;        // aracin tekerleklerinin MeshRenderer'lerinin bulundugu sinifi newliyoruz
    public WheelParticles wheelParticles;  // her tekerin cikaracagi duman particle larini ayri ayri tutan sinifi newliyoruz
   
    public float gasInput;       // gaza ne kadar bastigimizin bilgisini tutacak olan degisken
    public float brakeInput;     // frene ne kadar bastigimizin bilgisini tutacak olan degisken
    public float handBrakeInput;     // el frenine ne kadar bastigimizin bilgisini tutacak olan degisken
    public float steeringInput;  // direksiyonu ne kadar cevirdigimizin bilgisini tutacak olan degisken
    public float motorPower;     // aracin gucunu tutan degisken
    public float brakePower;     // aracin fren gucunu tutan degisken
    public float slipAngle;      // aracin kac derece kaydigi bilgisini tutan degisken
    public float steeringAngle;  // aracin dreksiyonunun kac derece dondugu bilgisini tutan degisken
    public float speed;          // aracin hiz bilgisini tutan degisken
    public float speedSmooth;    // speed degiskeni'nin ani degisimlerden etkilenmeyen hali (arac sesi icin kullaniyoruz)
    public float MaxSpeed;       // aracin maksimum hiz bilgisini tutan degisken

    public AnimationCurve steeringCurve; //arac hizli giderken daha az, yavas giderken daha cok direksiyon donus acisi olsun diye kullandigim degisken (100 birim hizla giderken 15 derece -
                                         // (1 birim hizla giderken 40 derece donus acisi olacak sekilde ayarlandi)
    public GameObject smokePrefab;       // icinde tekerlek dumani icin olusturulan ParticleSystem'in yer aldigi bir GameObject
    public Slider AutoGearSlider;        // otomatik vites için kullanilan bu slider 1, 2, 3, 4 olmak uzere sadece bu 4 int degeri alabilecek: 4(P), 3(R), 2(N), 1(D) viteslerini temsil edecek 
    private void Start()
    {
        rb = GetComponent<Rigidbody>();   
        InstantiateSmoke();
        AutoGearSlider.onValueChanged.AddListener(delegate { OnAutoGearValueChange(); }); // sliderin degeri her degistiginde, yani her vites degistiginde "OnAutoGearValueChange" fonk. tetiklenir
    }

    private void Update()
    {
        speed = wheelColliders.FRWheel.rpm * (wheelColliders.FRWheel.radius * (2f * Mathf.PI)) / 10f; // speed degiskenini aracin on tekerinin kac kez dondugunden aldigimiz veri ile hesapliyoruz
        speedSmooth = Mathf.Lerp(speedSmooth, speed, Time.deltaTime); // Lerp komutu sayesinde speed degiskeni'ni biraz geriden takip ediyor ve speed degiskeni'nin ani degisimlerinden etkilenmeyerek 
                                                                      // speedSmooth degerini yavas yavas arttiriyor veya azaltiyoruz (aracin sesi icin kullaniyoruz)

        CheckInput();
        ApplySteering();
        ApplyWheelPosition();
        CheckParticles();
    }

    private void OnAutoGearValueChange() // her vites degistiginde gasInput degerini "0" a esitliyoruz
    {
        gasInput = 0;
        ApplyMotorZeroTorque(); // tekerlere uygulanan torku 0 yapiyoruz 
    }
    private void InstantiateSmoke()  // Her tekerlek icin, icinde ParticleSystem bulunan bir GameObject olusturulup transformlari ayarlanip wheelParticles nesnesinin içerisi dolduruluyor
    {
        wheelParticles.FRWheelParticle = Instantiate(smokePrefab, wheelColliders.FRWheel.transform.position - Vector3.up * wheelColliders.FRWheel.radius, Quaternion.identity, 
            wheelColliders.FRWheel.transform).GetComponent<ParticleSystem>(); // Sag On teker icin, icerisinde ParticleSystem bulunan bir GameObject olusturulup, konumu tekerin en alti
            // olacak sekilde ayarlanip, Particlelerin tekerlek ile birlikte hareket etmesi için wheelColliders.FRWheel.transform'u kendisine parrent olarak ataniyor,
            // ve bu GameObject icerisindeki ParticleSystem Componenti alinip, wheelParticles.FRWheelParticle degiskeni icerisine ataniyor.
            // bu sayede wheelParticles.FRWheelParticle ParticleSystemi istenildigi zaman istenildigi yerden yonetilebilir

        wheelParticles.FLWheelParticle = Instantiate(smokePrefab, wheelColliders.FLWheel.transform.position - Vector3.up * wheelColliders.FRWheel.radius, Quaternion.identity,
            wheelColliders.FLWheel.transform).GetComponent<ParticleSystem>();

        wheelParticles.RRWheelParticle = Instantiate(smokePrefab, wheelColliders.RRWheel.transform.position - Vector3.up * wheelColliders.FRWheel.radius, Quaternion.identity,
            wheelColliders.RRWheel.transform).GetComponent<ParticleSystem>();

        wheelParticles.RLWheelParticle = Instantiate(smokePrefab, wheelColliders.RLWheel.transform.position - Vector3.up * wheelColliders.FRWheel.radius, Quaternion.identity,
            wheelColliders.RLWheel.transform).GetComponent<ParticleSystem>();
    }

    private void CheckInput() // oyuncudan alinan input verileri gerekli degiskenlere ataniyor
    {
        if(AutoGearSlider.value == 1)  // eger vites D konumunda ise true
        {
            if (Input.GetKey(KeyCode.W)) // "W" ye basili tutuldugu süre boyunca true
            {
                if (gasInput < 1) // direk tusa basildigi anda gasInput = 1 diyerek aracin cok sert kalkmasini engellemek icin, gasInput'u yavas yavas artiriyoruz
                {
                    gasInput += 2f * Time.deltaTime;
                }
                else // gas input'un 1 degerini gecmeyip sabit kalmasini sagliyoruz.
                {
                    gasInput = 1;
                }
                ApplyMotor();
            }
            else if (Input.GetKeyUp(KeyCode.W)) // elimizi "W" tusundan ceker cekmez gasInputu 0 a esitliyoruz
            {
                gasInput = 0;
                ApplyMotorZeroTorque();
            }
        }
        else if (AutoGearSlider.value == 3) // eger vites R konumunda ise true
        {
            if (Input.GetKey(KeyCode.W)) // "W" ye basili tutuldugu süre boyunca true
            {
                if (gasInput > -1) // direk tusa basildigi anda gasInput = -1 diyerek aracin cok sert geri geri gitmesini engellemek icin, gasInput'u yavas yavas azaltiyoruz
                {
                    gasInput -= 2f * Time.deltaTime;
                }
                else // gas input'un -1 degerinin altina inmeyip sabit kalmasini sagliyoruz.
                {
                    gasInput = -1;
                }
                ApplyMotor();
            }
            else if (Input.GetKeyUp(KeyCode.W)) // elimizi "W" tusundan ceker cekmez gasInputu 0 a esitliyoruz
            {
                gasInput = 0;
                ApplyMotorZeroTorque();
            }
        }

        if (Input.GetKey(KeyCode.S)) // "S" ye basili tutuldugu süre boyunca true
        {
            if (brakeInput < 1) // direk tusa basildigi anda brakeInput = 1 diyerek aracin cok sert durmasini engellemek icin, brakeInput'u yavas yavas artiriyoruz
            {
                brakeInput += 1f * Time.deltaTime;
            }
            else // brakeInput'un 1 degerini gecmeyip sabit kalmasini sagliyoruz.
            {
                brakeInput = 1;
            }
            ApplyNormalBrake();
        }

        if (Input.GetKeyUp(KeyCode.S)) // elimizi "S" tusundan ceker cekmez brakeInput'u 0 a esitliyoruz
        {
            brakeInput = 0;
            ApplyNormalBrake();
        }

        if (Input.GetKey(KeyCode.Space)) // "Space" ye basili tutuldugu süre boyunca true
        {
            handBrakeInput = 1;  // yavas yavas artirmak yerine "Space" basildigi anda handBrakeInput'u 1 yapiyoruz cunku el freni cekildigi anda arka tekerler kilitlensin istiyoruz
            ApplyHandBrake();
        }
        else if (Input.GetKeyUp(KeyCode.Space)) // elimizi "Space" tusundan ceker cekmez handBrakeInput'u 0 a esitliyoruz
        {
            handBrakeInput = 0;
            ApplyHandBrake();
        }

        steeringInput = Input.GetAxis("Horizontal"); // D tusuna basildigi zaman steeringInput degiskeni 1, A tusuna basildigi zaman -1 degerine gider
        slipAngle = Vector3.Angle(transform.forward, rb.velocity-transform.forward); // aracin kac derece kaydigi bilgisi ataniyor
    }

    private void ApplyNormalBrake() // oyuncu alýnan fren bilgisi tekerleklere uygulaniyor
    {
        wheelColliders.FRWheel.brakeTorque = brakeInput * brakePower * 0.7f; // fren gucunun yuzde 70'i on tekerlere uygulaniyor
        wheelColliders.FLWheel.brakeTorque = brakeInput * brakePower * 0.7f;

        wheelColliders.RRWheel.brakeTorque = brakeInput * brakePower * 0.3f; // fren gucunun yuzde 30'u arka tekerlere uygulaniyor
        wheelColliders.RLWheel.brakeTorque = brakeInput * brakePower * 0.3f;
    }

    private void ApplyHandBrake() // el freni tusuna basildigi zaman sadece arka iki tekere fren uygulaniyor
    {
        wheelColliders.RRWheel.brakeTorque = brakePower * handBrakeInput;
        wheelColliders.RLWheel.brakeTorque = brakePower * handBrakeInput;
    }

    private void ApplyMotor() // oyuncudan aldigimiz gasInput verisine gore motora (arka tekerlere) guc uyguluyoruz
    {
        if(speed < MaxSpeed) // eger hiz maksimum hizdan daha kucukse
        {
            wheelColliders.RRWheel.motorTorque = motorPower * gasInput; // aracin arka tekerlerine guc uygula
            wheelColliders.RLWheel.motorTorque = motorPower * gasInput;

        }
        else     // degilse
        {
            wheelColliders.RRWheel.motorTorque = 0; // aracin arka tekerlerine 0 guc uygula
            wheelColliders.RLWheel.motorTorque = 0;
            // aracin tekerine 0 guc uygula demek yerine else kismini hic yazmayip zaten eger speed < Maxspeed false dondururse araca guc uygulanmayacak diye dusunebilirisin ama
            // wheelColliders.RLWheel.motorTorque methodu kendisine uygulanan en son gucu aksi soylenmedikce surekli uygulamaya devam ediyor
            // dolayisi ile tekerlere guc uygulamayi kesmek icin 1 kez olsa bile wheelColliders.RLWheel.motorTorque = 0 demek zorundayiz
        }

    }
    private void ApplyMotorZeroTorque() // tekerlere daha fazla guc uygulamak istemedigimiz zaman 1 kez calistirmamiz gereken kod, cunku "wheelColliders.RRWheel.motorTorque" bu kod
        // sen anlik olarak cagirmasanda, en son aldigi degeri surekli tekerlere uyguluyor
    {
        wheelColliders.RRWheel.motorTorque = 0; // sag arka tekere 0 guc uygula
        wheelColliders.RLWheel.motorTorque = 0; // sol arka tekere 0 guc uygula
    }

    private void ApplySteering() // oyuncudan alinan direksiyon bilgisi once hiza gore hesaplanip sonra on tekerlere uygulaniyor
    {
        steeringAngle = steeringInput * steeringCurve.Evaluate(speed); // direksiyon acisini hiz degiskenine gore artirip azaltiyor (maksimum 40 derece - minimum 15 derece Inspectorden ayarlandi)
        if(AutoGearSlider.value == 1) // eger vites D konumunda ise
        {
            steeringAngle += Vector3.SignedAngle(transform.forward, rb.velocity + transform.forward, Vector3.up); // direksiyona counter acý veriyoruz. bunuda aracin gittigi yon ile baktigi yon 
                   // arasindaki aci farkini hesaplayarak ve bu farki on tekerleklere ters yonde uygulayarak yapiyoruz.
                   // Ayrica bunu sadece aracnin vitesi D konumunda ise yapiyoruz cunku arac geri geri giderken, gittigi yon ile baktigi yon bambaska ve geri geri giderken boyle bir sisteme ihtiyac duyulmuyor
            steeringAngle = Mathf.Clamp(steeringAngle, -40f, 40f); // direksiyon acisini -40 ile 40 arasina indiriyoruz
        }
        

        wheelColliders.FRWheel.steerAngle = steeringAngle; // elde ettigimiz degeri on tekerlere uyguluyoruz
        wheelColliders.FLWheel.steerAngle = steeringAngle;
    }

    private void ApplyWheelPosition() // UpdateWheel Fonksiyon'unu her teker icin ayri ayri cagiriyor
    {
        UpdateWheel(wheelColliders.FRWheel, wheelMeshes.FRWheelMesh);
        UpdateWheel(wheelColliders.FLWheel, wheelMeshes.FLWheelMesh);
        UpdateWheel(wheelColliders.RRWheel, wheelMeshes.RRWheelMesh);
        UpdateWheel(wheelColliders.RLWheel, wheelMeshes.RLWheelMesh);
    }

    private void CheckParticles() // Particlelerin olusma zamanini kontrol ediyoruz
    {
        WheelHit[] wheelHits = new WheelHit[4];                   // tekerlerin yere temas edip etmedigi bilgisini tutan degiskenn (Unity Ozelligi)
        wheelColliders.FRWheel.GetGroundHit(out wheelHits[0]);    // GetGroundHit metodu, WheelHit türünde bir degisken donduruyor, bu degiskenide wheelHit[] arrayinin icerisine atiyoruz
                                                                  // ***ONEMLI BILGI*** out keywordunu kullanmamin sebebi wheelHits[0] degiskeninin methodun icerisine bilgi tasiyan bir parametre degilde,
                                                                  // metodun disarisina bilgi cikaran bir parametre oldugunu belirtmek
        wheelColliders.FLWheel.GetGroundHit(out wheelHits[1]);

        wheelColliders.RRWheel.GetGroundHit(out wheelHits[2]);
        wheelColliders.RLWheel.GetGroundHit(out wheelHits[3]);

        float slipAllowance = 0.3f;   // tekerlerin particle sacmadan once kaymasina izin verilen miktar

        // asagida yazdigim kod icerisinde foreach kullanamadim cunku her tekerin particleSistemini wheelParticles.FRWheelParticle seklinde isimlendirdim.
        // eger particle sistemlerini 4 indexli bir array icerisinde tutsaydim foreach kullanabilirdim ve asagidaki gibi kod kalabaligi olmazdi
        // ama particle sistemlerini array icerisinde tanimlasaydim bu sefer kacinci indexin hangi tekerin particle sistemini temsil ettigini anlamak zor olurdu
        // yani array ile yapsaydým kodun okunabilirligi bence daha kotu olurdu 
        if ((Mathf.Abs(wheelHits[0].sidewaysSlip) + Mathf.Abs(wheelHits[0].forwardSlip) > slipAllowance)) // eger sag on tekerin yanal kayma degeri ile dikey kayma degerleri toplami izin verilen kayma degerinden buyukse 
        {
            wheelParticles.FRWheelParticle.Play(); // sag on teker particle yaymaya baslasin
        }
        else // degilse
        {
            wheelParticles.FRWheelParticle.Stop(); // sag on teker particle yaymayi durdursun
        }

        if ((Mathf.Abs(wheelHits[1].sidewaysSlip) + Mathf.Abs(wheelHits[1].forwardSlip) > slipAllowance)) // eger sol on tekerin yanal kayma degeri ile dikey kayma degerleri toplami izin verilen kayma degerinden buyukse 
        {
            wheelParticles.FLWheelParticle.Play(); // sol on teker particle yaymaya baslasin
        }
        else // degilse
        {
            wheelParticles.FLWheelParticle.Stop(); // sol on teker particle yaymayi durdursun
        }

        if ((Mathf.Abs(wheelHits[2].sidewaysSlip) + Mathf.Abs(wheelHits[2].forwardSlip) > slipAllowance)) // eger sag arka tekerin yanal kayma degeri ile dikey kayma degerleri toplami izin verilen kayma degerinden buyukse 
        {
            wheelParticles.RRWheelParticle.Play(); // sag arka teker particle yaymaya baslasin
        }
        else // degilse
        {
            wheelParticles.RRWheelParticle.Stop(); // sag arka teker particle yaymayi durdursun
        }

        if ((Mathf.Abs(wheelHits[3].sidewaysSlip) + Mathf.Abs(wheelHits[3].forwardSlip) > slipAllowance)) // eger sol arka tekerin yanal kayma degeri ile dikey kayma degerleri toplami izin verilen kayma degerinden buyukse 
        {
            wheelParticles.RLWheelParticle.Play(); // sol arka teker particle yaymaya baslasin
        }
        else // degilse
        {
            wheelParticles.RLWheelParticle.Stop(); // sol arka teker particle yaymayi durdursun
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
        float gas = Mathf.Clamp(gasInput, 0.5f, 1f); // gasinput degiskenini 0.5 ile 1 arasina daraltir
        return (speedSmooth / MaxSpeed) * gas; // normalize edilmis speed degiskeni ile gas degiskeni carpilarak return edilir.
                                               // speedSmooth: speed degiskenini biraz geriden takip eden, speed degiskeni'nin aksine ani degisikler yerine daha yumsak gecis degerleri alan degisken 
    }
}
