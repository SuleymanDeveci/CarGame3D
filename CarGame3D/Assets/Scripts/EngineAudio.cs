using UnityEngine;

public class EngineAudio : MonoBehaviour
{
    [Header("Running")] // Aracin giderken cikardigi ses ile alakali degiskenler
    [SerializeField] private AudioSource _runningAudio; // running AudioSource tanimliyoruz
    [SerializeField] private float _runningMaxVolume; // running AudioSource icin maksimum ses seviyesini tutan degisken
    [SerializeField] private float _runningMaxPitch;  // running AudioSurce icin maksimum pitch degerini tutan degisken

    [Header("Running")] // Devir kesici sesi ile alakali degiskenler
    [SerializeField] private float _limiterVolume;       // devir kesici ses yuksekligi
    [SerializeField] private float _limiterFrequency;    // devir kesici frekansi
    [SerializeField] private float _limiterEngageRatio = 0.9f;  // devir kesicinin hangi noktada devreye girecegini belirleyen degisken
    private float _revLimiterValue;     // devir kesici degeri (diger degiskenlerden gelen veriler ile yapilan hesaplamadan sonra dalgali bir deger alacak olan degisken)
    private float _speedRatio;  // hiz oranimizi tutan degisken: ne kadar hizli gittigimiz (0 - 1) * ne kadar gaza bastigimiz (0 - 1) = hiz oranimiz (0 - 1) arasinda  

    private CarController carController; // carController scriptini tanimliyoruz

    private void Start()
    {
        carController = GetComponent<CarController>(); // carController scriptini atiyoruz
    }

    private void Update()
    {
        if(carController != null) // eger carController scripti null ise, hata almamak icin null check yapiyoruz
        {
            _speedRatio = carController.GetSpeedRatio(); // hiz orani bilgisini aliyoruz
        }
        if(_speedRatio > _limiterEngageRatio) // devir kesici sesinin devreye girip girmemesi gerektigini kontrol ediyor
        {
            _revLimiterValue = (Mathf.Sin(Time.time * _limiterFrequency) + 1f) *_limiterVolume * (_speedRatio - _limiterEngageRatio); // _revLimiterValue degiskenine sesi dalgalandirmasi icin 0 ile 1 arasinda dalgali bir - 
                                                                                                                                 // float deger veriliyor
        }
        else
        {
            _revLimiterValue = 0f;   //devir kesici devrede deðilse, _revLimiterValue'yi 0 a eþitliyoruz
        }


        // Oncelikle Lerp nasil calisir: Mathf.Lerp(1, 100, 0.5) : 50 degerini dondurur
        // Oncelikle Lerp nasil calisir: Mathf.Lerp(1, 100, 0.25) : 25 degerini dondurur
        // Oncelikle Lerp nasil calisir: Mathf.Lerp(100, 1, 0.25) : 75 degerini dondurur
        // yani 3. parametre 0 a yaklastikca dondurdugu deger ilk parametreye yaklasir, 3. parametre 1 e yaklastikca dondurdugu deger 2. parametreye yaklasir
        
        _runningAudio.volume = Mathf.Lerp(0.3f, _runningMaxVolume, _speedRatio); // hiz oranimiz ne ise aracin sesinin o kadar yuksek cikmasini sagliyoruz
        _runningAudio.pitch = Mathf.Lerp(_runningAudio.pitch, Mathf.Lerp(0.3f, _runningMaxPitch, _speedRatio) +_revLimiterValue, Time.deltaTime); // ic ice lerp komutu ile aracin sesinin pitch degerini cok daha
                                                                                                                            // yumsak bir sekilde artirip azaltiyoruz

        //idleAudio.volume = Mathf.Lerp(idleMaxVolume, 0.1f, _speedRatio);  // idle sesini ayri bir ses olarak yapmamaya karar verdim. zaten running sesinin pitch degerini 0.3 yapinca idle sesi oluyor
        //idleAudio.pitch = Mathf.Lerp(1, idleMaxPitch, _speedRatio);
    }


}
