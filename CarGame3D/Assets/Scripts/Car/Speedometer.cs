using UnityEngine;

public class Speedometer : MonoBehaviour
{
    private Quaternion _needleRotate;       // gosterge ignesi icin rotation degerini tutan degisken
    [SerializeField] private CarController carController;    // CarControllerden hiz bilgisini almak icin nesnesini olusturuyoruz
    void FixedUpdate()
    {
        _needleRotate = Quaternion.Euler(0, 180, Mathf.Abs(carController.SpeedSmooth) * 1.5f);  // igneye uygulanacak olan rotation degerini hesapliyoruz
        transform.rotation = _needleRotate;      // hesapladigimiz degeri ignenin rotation degerine uyguluyoruz
    }
}
