using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GearController : MonoBehaviour
{
    public Slider AutoGearSlider { get; private set; }  // otomatik vites için kullanilan bu slider 1, 2, 3, 4 olmak uzere sadece bu 4 int degeri alabilecek: 4(P), 3(R), 2(N), 1(D) viteslerini temsil edecek 

    private void Awake()
    {
        AutoGearSlider = GameObject.FindGameObjectWithTag("AutoGearSlider").GetComponent<Slider>();   
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            ShiftUpForAutoGear();
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            ShiftDownForAutoGear();
        }
        
    }

    private void ShiftUpForAutoGear()   // otomatik vites icin vitesi yukari goturur
    {
        if(AutoGearSlider.value < 4)
        {
            AutoGearSlider.value++;
        }
        
    }

    private void ShiftDownForAutoGear()   // // otomatik vites icin vitesi asagi goturur
    {
        if (AutoGearSlider.value > 0)
        {
            AutoGearSlider.value--;
        }
    }
}
