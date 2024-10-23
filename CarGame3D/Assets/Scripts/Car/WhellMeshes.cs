using System;
using UnityEngine;

[Serializable]
public class WhellMeshes // bu sýnýf her tekerin MeshRenderer'ini ayrý ayrý tutuyor
{
    public MeshRenderer FRWheelMesh;
    public MeshRenderer FLWheelMesh;
    public MeshRenderer RRWheelMesh;
    public MeshRenderer RLWheelMesh;

    // burada icerisinde 4 tane MeshRenderer tutan bir array de tanimlayabilirdim ama o zaman erisim index ile olacakti ve kacinci index kacinci tekerin oldugu karisabilirdi
    // bu sekilde yapmamin okunabilirlik acisindan daha iyi oldugunu dusunuyorum
    // bu sekilde yapmamin olumsuz yonu 4 teker icinde uygulayacagim bir kodu foreach dongusu ile 1 satirda yazabilecekken 4 satirda yazmam gerekecek 
}
