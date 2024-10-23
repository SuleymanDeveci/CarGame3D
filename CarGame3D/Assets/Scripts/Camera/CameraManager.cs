using UnityEngine;

// ****************************************************************************** KODUN AMACI ***************************************************************************
/*  Cinemachine.FreeLook ozelligi, tanimli oldugu kamerada her zaman aktif olarak calisiyor. Ama ben bu ozelligin her zaman aktif olmasini istemiyorum. Sadece mouse'nin sag tusuna basili tuttugum sure boyunca
 *  aracin etrafina bakabilmeyi, diger durumlarda ise kameranin aracin arkasinda sabit durmasini istiyorum. Ama bu freeLook ozelligini bir input ile istedigim zaman aktif edip, istedigim zaman devre disi birakmanin 
 *  bir yolunu bulamadim. sanirim bunu yapmanin yolu, sahne icerisinde farkli kameralar tanimlayip, istenildigi zaman kameralar arasinda gecis yapmak. Ama bu kamera gecisi aracin cok hizli gittigi durumlarda
 *  kotu gozukuyor. Bu yuzden bende iki kamera kullanmaktan vazgectim ve freeLook kamerasinin x ve y eksenlerindeki maxSpeed (Sensitivity) degiskenini varsayilan olarak cok kucuk bir degere indirdim 
 *  ve sadece mouse'nin sag tusuna basildigi zaman normal degerlerini almasini sagladim. Bu sayede aslinda freeLook ozelligi her zaman aktif olmasina ragmen sadece tusa basildigi zaman etki edebiliyor
 *  diger durumlarda etkisi neredeyse 0'a iniyor
 */

public class CameraManager : MonoBehaviour
{
    public Cinemachine.CinemachineFreeLook freeLookCamera; // Cinemachine'nin freeLook ozelligini kullanmak icin nesnesini olusturuyoruz

    private void Start()
    {
        freeLookCamera.m_XAxis.m_MaxSpeed = 0.000001f;    // freeLook ozelliginin X ekseni hassasiyetini varsayilan olarak cok kucuk bir degere indiriyoruz
        freeLookCamera.m_YAxis.m_MaxSpeed = 0.0000001f;   // freeLook ozelliginin Y ekseni hassasiyetini varsayilan olarak cok kucuk bir degere indiriyoruz
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))  // eger mouse'nin sag tusuna basildi ise 1 kez calisir
        {
            freeLookCamera.m_XAxis.m_MaxSpeed = 2f;       // freeLook ozelliginin X ekseni hassasiyetini etrafa bakabilecegimiz kadar yuksek bir degere cikariyoruz
            freeLookCamera.m_YAxis.m_MaxSpeed = 0.06f;    // freeLook ozelliginin Y ekseni hassasiyetini etrafa bakabilecegimiz kadar yuksek bir degere cikariyoruz
        }

        if (Input.GetMouseButtonUp(1))   // eger mouse'nin sag tusu birakildi ise 1 kez calistir
        {
            freeLookCamera.m_XAxis.m_MaxSpeed = 0.000001f;    // artik tusa basmadigimiz icin freeLook ozelliginin X ekseni hassasiyetini cok kucuk bir degere indiriyoruz
            freeLookCamera.m_YAxis.m_MaxSpeed = 0.0000001f;   // artik tusa basmadigimiz icin freeLook ozelliginin Y ekseni hassasiyetini cok kucuk bir degere indiriyoruz
        }
    }
}
