using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Speedometer : MonoBehaviour
{
    public Quaternion needleRotate;       // 
    [SerializeField] private CarController carController;
    void FixedUpdate()
    {
        needleRotate = Quaternion.Euler(0, 180, Mathf.Abs(carController.SpeedSmooth) * 1.5f);
        transform.rotation = needleRotate;
    }
    
}
