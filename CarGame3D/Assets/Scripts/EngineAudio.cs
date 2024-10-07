using UnityEngine;

public class EngineAudio : MonoBehaviour
{
    [Header("Running")] // Running ile alakali degiskenler
    public AudioSource runningAudio; // running AudioSource tanimliyoruz
    public float runningMaxVolume; // running AudioSource icin maksimum ses seviyesini tutan degisken
    public float runningMaxPitch;  // running AudioSurce icin maksimum pitch degerini tutan degisken

    [Header("Idle")]  // Idle ile alakali degiskenler
    //public AudioSource idleAudio;  // idle AudioSource tanimliyoruz
    public float idleMaxPitch;
    public float idleMaxVolume;  // idle AudioSource icin maksimum ses seviyesini tutan degisken
    public float revLimiter;
    public float limiterVolume;
    public float limiterFrequency;
    public float limiterEngageRatio = 0.9f;
    public float speedRatio; // hiz oranimizi tutan degisken: ne kadar hizli gittigimiz (0 - 1) * ne kadar gaza bastigimiz (0 - 1) = hiz oranimiz (0 - 1) arasinda  

    private CarController carController; // carController scriptini tanimliyoruz

    private void Start()
    {
        carController = GetComponent<CarController>(); // carController scriptini atiyoruz
    }

    private void Update()
    {
        if(carController != null) // eger carController scripti null ise, hata almamak icin null check yapiyoruz
        {
            speedRatio = carController.GetSpeedRatio(); // hiz orani bilgisini aliyoruz
        }
        if(speedRatio > limiterEngageRatio)
        {
            revLimiter = (Mathf.Sin(Time.time * limiterFrequency) + 1f) *limiterVolume * (speedRatio - limiterEngageRatio);
        }


        // Oncelikle Lerp nasil calisir: Mathf.Lerp(1, 100, 0.5) : 50 degerini dondurur
        // Oncelikle Lerp nasil calisir: Mathf.Lerp(1, 100, 0.25) : 25 degerini dondurur
        // Oncelikle Lerp nasil calisir: Mathf.Lerp(100, 1, 0.25) : 75 degerini dondurur
        // yani 3. parametre 0 a yaklastikca dondurdugu deger ilk parametreye yaklasir, 3. parametre 1 e yaklastikca dondurdugu deger 2. parametreye yaklasir
        
        runningAudio.volume = Mathf.Lerp(0.3f, runningMaxVolume, speedRatio); // hiz oranimiz ne ise aracin sesinin o kadar yuksek cikmasini sagliyoruz
        runningAudio.pitch = Mathf.Lerp(runningAudio.pitch, Mathf.Lerp(0.3f, runningMaxPitch, speedRatio) +revLimiter, Time.deltaTime); // ic ice lerp komutu ile aracin sesinin pitch degerini cok daha
                                                                                                                            // yumsak bir sekilde artirip azaltiyoruz

        //idleAudio.volume = Mathf.Lerp(idleMaxVolume, 0.1f, speedRatio);  // idle sesini ayri bir ses olarak yapmamaya karar verdim. zaten running sesinin pitch degerini 0.3 yapinca idle sesi oluyor
        //idleAudio.pitch = Mathf.Lerp(1, idleMaxPitch, speedRatio);
    }


}
