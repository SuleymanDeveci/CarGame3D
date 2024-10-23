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
        if (Input.GetKeyDown(KeyCode.LeftShift))  // eger LeftShift tusuna basildiysa
        {
            ShiftUpForAutoGear();
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl))  // eger LeftControl tusuna basildiysa
        {
            ShiftDownForAutoGear();
        }
    }

    private void ShiftUpForAutoGear()   // otomatik vites icin vitesi yukari goturur 
    {
        if(AutoGearSlider.value < 4)  // degerin maksimum 4 olmasini saglar
        {
            AutoGearSlider.value++;  // AutoGearSlider'in degerini bir arttir
        }
    }

    private void ShiftDownForAutoGear()    // otomatik vites icin vitesi asagi goturur
    {
        if (AutoGearSlider.value > 0)    // degerin minimum 0 olmasini saglar
        {
            AutoGearSlider.value--;   // AutoGearSlider'in degerini bir azaltir
        }
    }
}
