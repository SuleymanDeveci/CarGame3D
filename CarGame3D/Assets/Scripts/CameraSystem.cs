using System.Collections;
using UnityEngine;

public class CameraSystem : MonoBehaviour
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
    
    public float sayi1, sayi2, sayi3, sayi4;
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
        sayi3 = Mathf.Clamp(sayi3, sayi1, sayi2);
        Debug.Log(sayi3);

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
            mousePositionDelta = Mathf.Clamp(mousePositionDelta, -5, 5);
            /*
            if (mousePositionDelta > 10)
            {
                mousePositionDelta = 10;
            }
            else if (mousePositionDelta < -10)
            {
                mousePositionDelta -= 10;
            }
            */

            /*  karekoklu etrafa bakma formulu
            if (mousePositionDelta > 0)
            {
                if (camNewPos.z < 0 && camNewPos.x <= 0)
                {
                    camNewPos = new Vector3(camNewPos.x -= (mousePositionDelta * mouseSensivity * Mathf.Sqrt((1920 - Screen.width))), camNewPos.y, camNewPos.z += (mousePositionDelta * mouseSensivity * Mathf.Sqrt((1920 - Screen.width))));
                    Debug.Log("1A");
                }
                else if (camNewPos.z > 0 && camNewPos.x < 0)
                {
                    camNewPos = new Vector3(camNewPos.x += (mousePositionDelta * mouseSensivity * Mathf.Sqrt((1920 - Screen.width))), camNewPos.y, camNewPos.z += (mousePositionDelta * mouseSensivity * Mathf.Sqrt((1920 - Screen.width))));
                    Debug.Log("1B");
                }
                else if (camNewPos.x > 0 && camNewPos.z > 0)
                {
                    camNewPos = new Vector3(camNewPos.x += (mousePositionDelta * mouseSensivity * Mathf.Sqrt((1920 - Screen.width))), camNewPos.y, camNewPos.z -= (mousePositionDelta * mouseSensivity * Mathf.Sqrt((1920 - Screen.width))));
                    Debug.Log("1C");
                }
                else if (camNewPos.z < 0 && camNewPos.x > 0)
                {
                    camNewPos = new Vector3(camNewPos.x -= (mousePositionDelta * mouseSensivity * Mathf.Sqrt((1920 - Screen.width))), camNewPos.y, camNewPos.z -= (mousePositionDelta * mouseSensivity * Mathf.Sqrt((1920 - Screen.width))));
                    Debug.Log("1D");
                }
            }
            */

            if (mousePositionDelta > 0)
            {
                if (camNewPos.z < 0 && camNewPos.x <= 0)
                {
                    camNewPos = new Vector3(camNewPos.x -= (mousePositionDelta * mouseSensivity), camNewPos.y, camNewPos.z += (mousePositionDelta * mouseSensivity));
                    Debug.Log("1A");
                }
                else if (camNewPos.z > 0 && camNewPos.x < 0)
                {
                    camNewPos = new Vector3(camNewPos.x += (mousePositionDelta * mouseSensivity * Mathf.Sqrt((1920 - Screen.width))), camNewPos.y, camNewPos.z += (mousePositionDelta * mouseSensivity * Mathf.Sqrt((1920 - Screen.width))));
                    Debug.Log("1B");
                }
                else if (camNewPos.x > 0 && camNewPos.z > 0)
                {
                    camNewPos = new Vector3(camNewPos.x += (mousePositionDelta * mouseSensivity * Mathf.Sqrt((1920 - Screen.width))), camNewPos.y, camNewPos.z -= (mousePositionDelta * mouseSensivity * Mathf.Sqrt((1920 - Screen.width))));
                    Debug.Log("1C");
                }
                else if (camNewPos.z < 0 && camNewPos.x > 0)
                {
                    camNewPos = new Vector3(camNewPos.x -= (mousePositionDelta * mouseSensivity * Mathf.Sqrt((1920 - Screen.width))), camNewPos.y, camNewPos.z -= (mousePositionDelta * mouseSensivity * Mathf.Sqrt((1920 - Screen.width))));
                    Debug.Log("1D");
                }
            }
            else
            {
                if (camNewPos.z < 0 && camNewPos.x >= 0)
                {
                    camNewPos = new Vector3(camNewPos.x += (-mousePositionDelta * mouseSensivity * Mathf.Sqrt((1920 - Screen.width))), camNewPos.y, camNewPos.z += (-mousePositionDelta * mouseSensivity * Mathf.Sqrt((1920 - Screen.width))));
                    Debug.Log("2A");
                }
                else if (camNewPos.z > 0 && camNewPos.x > 0)
                {
                    camNewPos = new Vector3(camNewPos.x -= (-mousePositionDelta * mouseSensivity * Mathf.Sqrt((1920 - Screen.width))), camNewPos.y, camNewPos.z += (-mousePositionDelta * mouseSensivity * Mathf.Sqrt((1920 - Screen.width))));
                    Debug.Log("2B");
                }
                else if (camNewPos.z > 0 && camNewPos.x < 0)
                {
                    camNewPos = new Vector3(camNewPos.x -= (-mousePositionDelta * mouseSensivity * Mathf.Sqrt((1920 - Screen.width))), camNewPos.y, camNewPos.z -= (-mousePositionDelta * mouseSensivity * Mathf.Sqrt((1920 - Screen.width))));
                    Debug.Log("2C");
                }
                else if (camNewPos.z < 0 && camNewPos.x < 0)
                {
                    camNewPos = new Vector3(camNewPos.x += (-mousePositionDelta * mouseSensivity * Mathf.Sqrt((1920 - Screen.width))), camNewPos.y, camNewPos.z -= (-mousePositionDelta * mouseSensivity * Mathf.Sqrt((1920 - Screen.width))));
                    Debug.Log("2D");
                }
            }
        }

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
