using UnityEngine;

public class CarController : MonoBehaviour
{
    private Rigidbody rb;
    public WheelColliders wheelColliders;  // aracýn tekerlek colliderlerinin bulunduðu sinifi newliyoruz
    public WhellMeshes wheelMeshes;        // aracýn tekerleklerinin MeshRenderer'lerinin bulundugu sinifi newliyoruz
    public WheelParticles wheelParticles;  // her tekerin cikaracagi duman particle larini ayri ayri tutan sinifi newliyoruz
    public float gasInput;       // gaza ne kadar bastigimizin bilgisini tutacak olan degisken
    public float brakeInput;     // frene ne kadar bastigimizin bilgisini tutacak olan degisken
    public float steeringInput;  // direksiyonu ne kadar cevirdigimizin bilgisini tutacak olan degisken
    public float motorPower;     // aracin gucunu tutan degisken
    public float brakePower;     // aracin fren gucunu tutan degisken
    public float slipAngle;      // aracin kac derece kaydigi bilgisini tutan degisken
    public float steeringAngle;  // aracin dreksiyonunun kac derece dondugu bilgisini tutan degisken
    public float speed;          // aracin hizini gosteren degisken
    public AnimationCurve steeringCurve; //arac hizli giderken daha az, yavas giderken daha cok direksiyon donus acisi olsun diye kullandigim degisken (100 birim hizla giderken 15 derece -
                                         // (1 birim hizla giderken 40 derece donus acisi olacak sekilde ayarlandi)
    public GameObject smokePrefab;       // icinde tekerlek dumani icin olusturulan ParticleSystem'in yer aldigi bir GameObject
    private void Start()
    {
        rb = GetComponent<Rigidbody>();   
        InstantiateSmoke();    
    }

    private void Update()
    {
        speed = rb.velocity.magnitude; // speed degiskeni aracin anlik hizi ile surekli guncelleniyor.
        ApplyWheelPosition();
        ApplyMotor();
        ApplySteering();
        ApplyBrake();
        CheckInput();
    }

    private void InstantiateSmoke()  // Her tekerlek icin, icinde ParticleSystem bulunan bir GameObject olusturulup transformlari ayarlanip wheelParticles nesnesinin içerisi dolduruluyor
    {
        wheelParticles.FRWheelParticle = Instantiate(smokePrefab, wheelColliders.FRWheel.transform.position - Vector3.up * wheelColliders.FRWheel.radius, Quaternion.identity, 
            wheelColliders.FRWheel.transform).GetComponent<ParticleSystem>(); // Sag On teker icin, icerisinde ParticleSystem bulunan bir GameObject olusturulup, konumu tekerin en alti
            // olacak sekilde ayarlanip, icerisindeki ParticleSystem Componenti alinip, wheelParticles.FRWheelParticle degiskeni icerisine atandi.

        wheelParticles.FLWheelParticle = Instantiate(smokePrefab, wheelColliders.FLWheel.transform.position - Vector3.up * wheelColliders.FRWheel.radius, Quaternion.identity,
            wheelColliders.FLWheel.transform).GetComponent<ParticleSystem>();

        wheelParticles.RRWheelParticle = Instantiate(smokePrefab, wheelColliders.RRWheel.transform.position - Vector3.up * wheelColliders.FRWheel.radius, Quaternion.identity,
            wheelColliders.RRWheel.transform).GetComponent<ParticleSystem>();

        wheelParticles.RLWheelParticle = Instantiate(smokePrefab, wheelColliders.RLWheel.transform.position - Vector3.up * wheelColliders.FRWheel.radius, Quaternion.identity,
            wheelColliders.RLWheel.transform).GetComponent<ParticleSystem>();
    }

    private void CheckInput() // kullanicidan alinan input verileri gerekli degiskenlere ataniyor
    {
        gasInput = Input.GetAxis("Vertical"); // W ye basili tutuldugunda gasInput degiskeni 1 degerine gider, S ye basildiginda -1 degerine gider
        steeringInput = Input.GetAxis("Horizontal"); // D tusuna basildigi zaman steeringInput degiskeni 1, A tusuna basildigi zaman -1 degerine gider
        slipAngle = Vector3.Angle(transform.forward, rb.velocity-transform.forward); 
        if(slipAngle < 120)
        {
            if(gasInput < 0)
            {
                brakeInput = Mathf.Abs(gasInput);
                gasInput = 0;
            }
            else
            {
                brakeInput = 0;
            }
        }
        else
        {
            brakeInput = 0;
        }
    }

    private void ApplyBrake()
    {
        wheelColliders.FRWheel.brakeTorque = brakeInput * brakePower * 0.7f;
        wheelColliders.FLWheel.brakeTorque = brakeInput * brakePower * 0.7f;

        wheelColliders.RRWheel.brakeTorque = brakeInput * brakePower * 0.3f;
        wheelColliders.RLWheel.brakeTorque = brakeInput * brakePower * 0.3f;
    }


    private void ApplyMotor()
    {
        wheelColliders.RRWheel.motorTorque = motorPower * gasInput;
        wheelColliders.RLWheel.motorTorque = motorPower * gasInput;
    }

    private void ApplySteering()
    {
        steeringAngle = steeringInput * steeringCurve.Evaluate(speed);
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
