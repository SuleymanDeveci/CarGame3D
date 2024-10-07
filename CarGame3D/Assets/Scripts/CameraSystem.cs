using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSystem : MonoBehaviour
{
    public Transform player;
    private Rigidbody rb;
    public Vector3 offSet;
    public Vector3 offSet2;
    public float speed;


    public float mousePosition1;
    public float mousePosition2;
    public float mousePositionDelta;
    public bool isFirstClick;
    public float mouseSensivity;
    void Start()
    {
        rb = player.GetComponent<Rigidbody>();
        isFirstClick = true;
        offSet = offSet2;
    }

    private void LateUpdate()
    { 
        transform.position = Vector3.Lerp(transform.position, player.position + player.transform.TransformVector(offSet), speed * Time.deltaTime);

        transform.LookAt(player);
    }
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            if (isFirstClick == true)
            {
                mousePosition1 = Input.mousePosition.x;
                mousePosition2 = Input.mousePosition.x;
                isFirstClick = false;
            }
            StartCoroutine(MousePosDelay());

            mousePositionDelta = mousePosition1 - mousePosition2;
            //Debug.Log(mousePositionDelta);
            if(mousePositionDelta > 0)
            {
                if (offSet.z < 0 && offSet.x <= 0)
                {
                    offSet = new Vector3(offSet.x -= (mousePositionDelta * mouseSensivity), offSet.y, offSet.z += (mousePositionDelta * mouseSensivity));
                    Debug.Log("1A");
                }
                else if (offSet.z > 0 && offSet.x < 0)
                {
                    offSet = new Vector3(offSet.x += (mousePositionDelta * mouseSensivity), offSet.y, offSet.z += (mousePositionDelta * mouseSensivity));
                    Debug.Log("1B");
                }
                else if (offSet.x > 0 && offSet.z > 0)
                {
                    offSet = new Vector3(offSet.x += (mousePositionDelta * mouseSensivity), offSet.y, offSet.z -= (mousePositionDelta * mouseSensivity));
                    Debug.Log("1C");
                }
                else if (offSet.z < 0 && offSet.x > 0)
                {
                    offSet = new Vector3(offSet.x -= (mousePositionDelta * mouseSensivity), offSet.y, offSet.z -= (mousePositionDelta * mouseSensivity));
                    Debug.Log("1D");
                }
            }
            else
            {
                if (offSet.z < 0 && offSet.x >= 0)
                {
                    offSet = new Vector3(offSet.x += (-mousePositionDelta * mouseSensivity), offSet.y, offSet.z += (-mousePositionDelta * mouseSensivity));
                    Debug.Log("2A");
                }
                else if (offSet.z > 0 && offSet.x > 0)
                {
                    offSet = new Vector3(offSet.x -= (-mousePositionDelta * mouseSensivity), offSet.y, offSet.z += (-mousePositionDelta * mouseSensivity));
                    Debug.Log("2B");
                }
                else if (offSet.z > 0 && offSet.x < 0)
                {
                    offSet = new Vector3(offSet.x -= (-mousePositionDelta * mouseSensivity), offSet.y, offSet.z -= (-mousePositionDelta * mouseSensivity));
                    Debug.Log("2C");
                }
                else if (offSet.z < 0 && offSet.x < 0)
                {
                    offSet = new Vector3(offSet.x += (-mousePositionDelta * mouseSensivity), offSet.y, offSet.z -= (-mousePositionDelta * mouseSensivity));
                    Debug.Log("2D");
                }
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isFirstClick = true;
            offSet = offSet2;
        }
    }

    IEnumerator MousePosDelay()
    {
        mousePosition1 = Input.mousePosition.x;
        yield return new WaitForSeconds(0.05f);
        mousePosition2 = Input.mousePosition.x;
    }
}
