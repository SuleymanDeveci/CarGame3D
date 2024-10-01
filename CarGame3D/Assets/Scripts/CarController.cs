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
    public float speed;          // aracin hizini gosteren degisken
    public AnimationCurve steeringCurve; //arac hizli giderken daha az, yavas giderken daha cok direksiyon donus acisi olsun diye kullandigim degisken (100 birim hizla giderken 15 derece -
                                         // (1 birim hizla giderken 40 derece donus acisi olacak sekilde ayarlandi)
    public GameObject smokePrefab;       // icinde tekerlek dumani icin olusturulan ParticleSystem'in yer aldigi bir GameObject
    public Slider AutoGearSlider;        // otomatik vites için kullanilan bu slider 1, 2, 3, 4 olmak üzere sadece bu 4 int degeri alabilecek: 4(P), 3(R), 2(N), 1(D) viteslerini temsil edecek 
    private void Start()
    {
        rb = GetComponent<Rigidbody>();   
        InstantiateSmoke();
        AutoGearSlider.onValueChanged.AddListener(delegate { OnAutoGearValueChange(); }); // sliderin degeri degistiginde, yani vites degistiginde "OnAutoGearValueChange" fonk. tetiklenir
    }

    private void Update()
    {
        speed = rb.velocity.magnitude * 5; // speed degiskeni aracin anlik hizi ile surekli guncelleniyor.
        ApplyWheelPosition();
        ApplyMotor();
        ApplySteering();
        CheckInput();
    }

    private void OnAutoGearValueChange() // her vites degistiginde gasInput degerini "0" a esitliyoruz
    {
        gasInput = 0;
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
                    gasInput += 0.1f * Time.deltaTime;
                }
                else // gas input'un 1 degerini gecmeyip sabit kalmasini sagliyoruz.
                {
                    gasInput = 1;
                }
            }
            else if (Input.GetKeyUp(KeyCode.W)) // elimizi "W" tusundan ceker cekmez gasInputu 0 a esitliyoruz
            {
                gasInput = 0;
            }
        }
        else if (AutoGearSlider.value == 3) // eger vites R konumunda ise true
        {
            if (Input.GetKey(KeyCode.W)) // "W" ye basili tutuldugu süre boyunca true
            {
                if (gasInput > -1) // direk tusa basildigi anda gasInput = -1 diyerek aracin cok sert geri geri gitmesini engellemek icin, gasInput'u yavas yavas azaltiyoruz
                {
                    gasInput -= 0.1f * Time.deltaTime;
                }
                else // gas input'un -1 degerinin altina inmeyip sabit kalmasini sagliyoruz.
                {
                    gasInput = -1;
                }
            }
            else if (Input.GetKeyUp(KeyCode.W)) // elimizi "W" tusundan ceker cekmez gasInputu 0 a esitliyoruz
            {
                gasInput = 0;
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
        wheelColliders.RRWheel.motorTorque = motorPower * gasInput;
        wheelColliders.RLWheel.motorTorque = motorPower * gasInput;
    }

    private void ApplySteering() // oyuncudan alinan direksiyon bilgisi once hiza gore hesaplanip sonra on tekerlere uygulaniyor
    {
        steeringAngle = steeringInput * steeringCurve.Evaluate(speed); // direksiyon acisini hiz degiskenine gore artirip azaltiyor (maksimum 40 derece - minimum 15 derece Inspectorden ayarlandi)
        wheelColliders.FRWheel.steerAngle = steeringAngle;
        wheelColliders.FLWheel.steerAngle = steeringAngle;
    }

    private void ApplyWheelPosition()
    {
        UpdateWheel(wheelColliders.FRWheel, wheelMeshes.FRWheelMesh);
        UpdateWheel(wheelColliders.FLWheel, wheelMeshes.FLWheelMesh);
        UpdateWheel(wheelColliders.RRWheel, wheelMeshes.RRWheelMesh);
        UpdateWheel(wheelColliders.RLWheel, wheelMeshes.RLWheelMesh);
    }

    private void UpdateWheel(WheelCollider coll, MeshRenderer wheelMesh)
    {
        Quaternion quat;
        Vector3 position;
        coll.GetWorldPose(out position, out quat);
        wheelMesh.transform.position = position;
        wheelMesh.transform.rotation = quat;
    }
}
