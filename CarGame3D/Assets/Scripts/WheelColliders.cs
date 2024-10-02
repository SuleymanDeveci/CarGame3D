using System;
using UnityEngine;

[Serializable]
public class WheelColliders // bu sýnýf her tekerin collider'ini ayrý ayrý tutuyor
{
    public WheelCollider FRWheel;
    public WheelCollider FLWheel;
    public WheelCollider RRWheel;
    public WheelCollider RLWheel;

    // burada icerisinde 4 tane WheelCollider tutan bir array de tanimlayabilirdim ama o zaman erisim index ile olacakti ve kacinci index kacinci tekerin oldugu karisabilirdi
    // bu sekilde yapmamin okunabilirlik acisindan daha iyi oldugunu dusunuyorum
    // bu sekilde yapmamin olumsuz yonu 4 teker icinde uygulayacagim bir kodu foreach dongusu ile 1 satirda yazabilecekken 4 satirda yazmam gerekecek 
}
