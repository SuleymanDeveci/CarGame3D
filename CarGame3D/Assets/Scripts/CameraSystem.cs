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
public class FollowCamera : MonoBehaviour
{
    public Transform followTarget;    // cameranin bakacagi objenin transformunu aliyoruz
    public Vector3 camNewPos;       // kameranin yavasca gidecegi yeni pozisyonu
    public Vector3 camDefaultPos;
    public float speed;

    
    public float mousePosition1;
    public float mousePosition2;
    public float mousePositionDelta;
    public bool isFirstClick;
    public float mouseSensivity;
    
    void Start()
    {
        isFirstClick = true;
        camNewPos = camDefaultPos;
    }

    private void LateUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, followTarget.position + followTarget.transform.TransformVector(camNewPos), speed * Time.deltaTime);
        transform.LookAt(followTarget);
    }
    
    void Update()
    {
        
        if (Input.GetMouseButton(1))
        {
            if (isFirstClick == true)
            {
                mousePosition1 = Input.mousePosition.x;
                mousePosition2 = Input.mousePosition.x;
                isFirstClick = false;
            }
            StartCoroutine(MousePosDelay());

            mousePositionDelta = mousePosition1 - mousePosition2;

            if (mousePositionDelta > 0)
            {
                if (camNewPos.z < 0 && camNewPos.x <= 0)
                {
                    camNewPos = new Vector3(camNewPos.x -= (mousePositionDelta * mouseSensivity * 1/Screen.width), camNewPos.y, camNewPos.z += (mousePositionDelta * mouseSensivity * 1 / Screen.width));
                    Debug.Log("1A");
                }
                else if (camNewPos.z > 0 && camNewPos.x < 0)
                {
                    camNewPos = new Vector3(camNewPos.x += (mousePositionDelta * mouseSensivity * 1 / Screen.width), camNewPos.y, camNewPos.z += (mousePositionDelta * mouseSensivity * 1 / Screen.width));
                    Debug.Log("1B");
                }
                else if (camNewPos.x > 0 && camNewPos.z > 0)
                {
                    camNewPos = new Vector3(camNewPos.x += (mousePositionDelta * mouseSensivity * 1 / Screen.width), camNewPos.y, camNewPos.z -= (mousePositionDelta * mouseSensivity * 1 / Screen.width));
                    Debug.Log("1C");
                }
                else if (camNewPos.z < 0 && camNewPos.x > 0)
                {
                    camNewPos = new Vector3(camNewPos.x -= (mousePositionDelta * mouseSensivity * 1 / Screen.width), camNewPos.y, camNewPos.z -= (mousePositionDelta * mouseSensivity * 1 / Screen.width));
                    Debug.Log("1D");
                }
            }
            else
            {
                if (camNewPos.z < 0 && camNewPos.x >= 0)
                {
                    camNewPos = new Vector3(camNewPos.x += (-mousePositionDelta * mouseSensivity * 1 / Screen.width), camNewPos.y, camNewPos.z += (-mousePositionDelta * mouseSensivity * 1 / Screen.width));
                    Debug.Log("2A");
                }
                else if (camNewPos.z > 0 && camNewPos.x > 0)
                {
                    camNewPos = new Vector3(camNewPos.x -= (-mousePositionDelta * mouseSensivity * 1 / Screen.width), camNewPos.y, camNewPos.z += (-mousePositionDelta * mouseSensivity * 1 / Screen.width));
                    Debug.Log("2B");
                }
                else if (camNewPos.z > 0 && camNewPos.x < 0)
                {
                    camNewPos = new Vector3(camNewPos.x -= (-mousePositionDelta * mouseSensivity * 1 / Screen.width), camNewPos.y, camNewPos.z -= (-mousePositionDelta * mouseSensivity * 1 / Screen.width));
                    Debug.Log("2C");
                }
                else if (camNewPos.z < 0 && camNewPos.x < 0)
                {
                    camNewPos = new Vector3(camNewPos.x += (-mousePositionDelta * mouseSensivity * 1 / Screen.width), camNewPos.y, camNewPos.z -= (-mousePositionDelta * mouseSensivity * 1 / Screen.width));
                    Debug.Log("2D");
                }
            }
        }
        
        transform.position = Vector3.Lerp(transform.position, followTarget.position + followTarget.transform.TransformVector(camNewPos), speed * Time.deltaTime);

        transform.LookAt(followTarget);
        
        if (Input.GetMouseButtonUp(1))
        {
            isFirstClick = true;
            camNewPos = camDefaultPos;
        }
    }

    IEnumerator MousePosDelay()
    {
        mousePosition1 = Input.mousePosition.x;
        yield return new WaitForSeconds(0.05f);
        mousePosition2 = Input.mousePosition.x;
    }
    
}
