using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Cinemachine.CinemachineFreeLook freeLookCamera;

    private void Start()
    {
        freeLookCamera.m_XAxis.m_MaxSpeed = 0.000001f;
        freeLookCamera.m_YAxis.m_MaxSpeed = 0.0000001f;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            freeLookCamera.m_XAxis.m_MaxSpeed = 2f;
            freeLookCamera.m_YAxis.m_MaxSpeed = 0.06f;
        }

        if (Input.GetMouseButtonUp(1))
        {
            freeLookCamera.m_XAxis.m_MaxSpeed = 0.000001f;
            freeLookCamera.m_YAxis.m_MaxSpeed = 0.0000001f;
        }
    }
}
