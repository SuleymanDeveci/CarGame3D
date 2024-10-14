using System.Collections;
using UnityEngine;

/*****************  KAMERANIN HEM ARACI TAKIP ETMESINI SAGLAYAN HEMDE MOUSE ILE ARACIN ETRAFINA BAKABILMEMIZI SAGLAYAN KOD  ******************************
 *  
 *  kendimi geliþtirmek için, Cinemachine kullanmak yerine, kamerayi kendi yazdigim kod ile kontrol etmek istedim ve yaptim da, asagida yazdigim bu kod sayesinde kamera hem yumusak
 *  bir sekilde araci takip ediyor hemde mouse ile aracin etradina bakabiliyorduk. Ancak arac cok hizlandigi zaman mouse ile aracin etrafina bakmaya calisinca ise yaramiyor kamera
 *  hemen aracin arkasina gecmek istiyordu. Bu sorunu cozmek icin bir sure ugrastiktan sonra, daha fazla zaman kaybetmek istemedigim icin Cinemachine'nin freeLook ozelligini kullandim
 *  
 *  Bu kodu suan oyunda kullanilmamasina ragmen daha sonra tekrar incelemek isterim diye silmedim. 
 */
public class CameraSystem : MonoBehaviour
{
    public Transform followTarget;    // kameranin bakacagi objenin transformu
    private Vector3 _camNewPos;       // kameranin yavasca gidecegi yeni pozisyonu
    public Vector3 camDefaultPos;   // kameranin varsayilan pozisyonu
    public float camFollowSpeed;    // kameranin araci takip etme yumsakligi 

    private float _mousePosition1;     // Mouse'nin ilk pozisyonu (sadece x ekseni)
    private float _mousePosition2;      // Mouse'nin ilk pozisyondan 0.05f saniye sonra bulundugu ikinci pozisyonu (sadece x ekseni)
    private float _mousePositionDelta;  // Mouse'nin ilk pozisyonundan ikinci pozisyonunu cikararak elde ettigimiz ve bize mouse'nin hangi yone, hangi hizla gittigi bilgisini veren degisken (sadece x ekseni)
    public float mouseSensitivity;      // free look modunda mouse hassasiyetini ayarlayan degisken
    
    void Start()
    {
        _camNewPos = camDefaultPos; // baslangicta kamerayi varsayilan pozisyona getiriyoruz 
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, followTarget.position + followTarget.transform.TransformVector(_camNewPos), camFollowSpeed * Time.deltaTime); // lerp metodu sayesinde kameranin istedigimiz
            // konuma gitmesini sagliyoruz. ama burada "followTarget.position" kodu araci takip etmeye yararken, "followTarget.transform.TransformVector(_camNewPos)" kodu "_camNewPos" degiskenine verilen yeni 
            // Vector3 degeri sayesinde kamera'nin, arac'in etrafina bakabilmesini sagliyor. 

        transform.LookAt(followTarget); // kamera'nin followTarget'a bakmasini saglar
    }
    
    void Update()
    {
        
        if (Input.GetMouseButtonDown(1))  // eger mouse'nin sag tusuna basildi ise 1 kez calisir
        {
            _mousePosition1 = Input.mousePosition.x; // mouse'nin suanki konumunun x degerini _mousePosition1 degiskeninin icerisine atar
            _mousePosition2 = Input.mousePosition.x; // mouse'nin suanki konumunun x degerini _mousePosition2 degiskeninin icerisine atar

            StartCoroutine(MousePosDelay());  // mouse'nin ikinci konumunu almak icin 0.05 saniye bekleten kod. 0.05 saniye bekledikten sonra kodun icerisinde atama yapilir

            _mousePositionDelta = _mousePosition1 - _mousePosition2; // mouse'nin birinci pozisyonu ve ikinci pozisyonu arasindaki fark hesaplanir. cikan sonuc bize mouse'nin hangi yonde ve hangi hizda gittigi bilgisini verir

            if (_mousePositionDelta > 0)  // eger mouse ekranin solundan sagina dogru gidiyor ise : kamera aracin sol tarafina dogru donmeli
            {

                // asagida bulunan 4 adet if else blogunun temel olarak yaptigi sey, _camNewPos Vector'une yeni bir deger atamak, boylelikle yeni atanan deger dogrultusunda kamera aracin etrafinda doner.
                // Mesela _camNewPos degerini varsayilan olarak (0, 0, -10) olarak tanimladik. bu da kameranin araci tam arkasindan bakmasini sagliyor, eger bu deger (0, 0, 10) olursa kamera araca On taraftan bakar
                // eger bu deger (0, 10, 0) olursa kamera araca sag tarafindan bakar ve eger bu deger (0, -10, 0) olursa kamera araca sol tarafindan bakar.
                // asagidaki 4 adet if else kodu ise bu degiskenin mouse hareketi dogrultusunda gereken degeri almasini sagliyor
                if (_camNewPos.z < 0 && _camNewPos.x <= 0)
                {
                    _camNewPos = new Vector3(_camNewPos.x -= (_mousePositionDelta * mouseSensitivity * (1 / Screen.width)), _camNewPos.y, _camNewPos.z += (_mousePositionDelta * mouseSensitivity * (1 / Screen.width)));
                    Debug.Log("1A");
                }
                else if (_camNewPos.z > 0 && _camNewPos.x < 0)
                {
                    _camNewPos = new Vector3(_camNewPos.x += (_mousePositionDelta * mouseSensitivity * (1 / Screen.width)), _camNewPos.y, _camNewPos.z += (_mousePositionDelta * mouseSensitivity * (1 / Screen.width)));
                    Debug.Log("1B");
                }
                else if (_camNewPos.x > 0 && _camNewPos.z > 0)
                {
                    _camNewPos = new Vector3(_camNewPos.x += (_mousePositionDelta * mouseSensitivity * (1 / Screen.width)), _camNewPos.y, _camNewPos.z -= (_mousePositionDelta * mouseSensitivity * (1 / Screen.width)));
                    Debug.Log("1C");
                }
                else if (_camNewPos.z < 0 && _camNewPos.x > 0)
                {
                    _camNewPos = new Vector3(_camNewPos.x -= (_mousePositionDelta * mouseSensitivity * (1 / Screen.width)), _camNewPos.y, _camNewPos.z -= (_mousePositionDelta * mouseSensitivity * (1 / Screen.width)));
                    Debug.Log("1D");
                }
            }
            else // eger mouse ekranin sagindan soluna dogru gidiyor ise : kamera aracin sag tarafina dogru donmeli
            {
                if (_camNewPos.z < 0 && _camNewPos.x >= 0)
                {
                    _camNewPos = new Vector3(_camNewPos.x += (-_mousePositionDelta * mouseSensitivity * (1 / Screen.width)), _camNewPos.y, _camNewPos.z += (-_mousePositionDelta * mouseSensitivity * (1 / Screen.width)));
                    Debug.Log("2A");
                }
                else if (_camNewPos.z > 0 && _camNewPos.x > 0)
                {
                    _camNewPos = new Vector3(_camNewPos.x -= (-_mousePositionDelta * mouseSensitivity * (1 / Screen.width)), _camNewPos.y, _camNewPos.z += (-_mousePositionDelta * mouseSensitivity * (1 / Screen.width)));
                    Debug.Log("2B");
                }
                else if (_camNewPos.z > 0 && _camNewPos.x < 0)
                {
                    _camNewPos = new Vector3(_camNewPos.x -= (-_mousePositionDelta * mouseSensitivity * (1 / Screen.width)), _camNewPos.y, _camNewPos.z -= (-_mousePositionDelta * mouseSensitivity * (1 / Screen.width)));
                    Debug.Log("2C");
                }
                else if (_camNewPos.z < 0 && _camNewPos.x < 0)
                {
                    _camNewPos = new Vector3(_camNewPos.x += (-_mousePositionDelta * mouseSensitivity * (1 / Screen.width)), _camNewPos.y, _camNewPos.z -= (-_mousePositionDelta * mouseSensitivity * (1 / Screen.width)));
                    Debug.Log("2D");
                }
            }
        }
        
        transform.position = Vector3.Lerp(transform.position, followTarget.position + followTarget.transform.TransformVector(_camNewPos), camFollowSpeed * Time.deltaTime); // yukarida hesaplanan camNewPos degerini 
                   // kameranin pozisyonuna uyguluyor

        transform.LookAt(followTarget);  // kameranin her zaman araca bakmasini sagliyor
        
        if (Input.GetMouseButtonUp(1)) // eger mouse'nin sag tusu birakildi ise 1 kez calisir (free look modundan cýk)
        {
            _camNewPos = camDefaultPos; // kameranin pozisyonunu varsayilan pozisyona getiriyor
        }
    }

    IEnumerator MousePosDelay()  // mouse'nin pozisyonunu 0.05 saniye aralik ile 2 farkli degiskene kaydeden fonksiyon
    {
        _mousePosition1 = Input.mousePosition.x; // mouse'nin ilk pozisyonunu _mousePosition1 degiskenine atiyor
        yield return new WaitForSeconds(0.05f);  // bekleme suresini ayarlayan kod
        _mousePosition2 = Input.mousePosition.x; // mouse'nin ikinci pozisyonunu _mousePosition2 degiskenine atiyor
    }
}
