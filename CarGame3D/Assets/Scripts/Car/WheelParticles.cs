using System;
using UnityEngine;

[Serializable]
public class WheelParticles // bu sýnýf her tekerin ParticleSystem'ini ayrý ayrý tutuyor
{
    public ParticleSystem FRWheelParticle;
    public ParticleSystem FLWheelParticle;
    public ParticleSystem RRWheelParticle;
    public ParticleSystem RLWheelParticle;

    // burada icerisinde 4 tane ParticleSystem tutan bir array de tanimlayabilirdim ama o zaman erisim index ile olacakti ve kacinci index kacinci tekerin oldugu karisabilirdi
    // bu sekilde yapmamin okunabilirlik acisindan daha iyi oldugunu dusunuyorum
    // bu sekilde yapmamin olumsuz yonu 4 teker icinde uygulayacagim bir kodu foreach dongusu ile 1 satirda yazabilecekken 4 satirda yazmam gerekecek 
}
