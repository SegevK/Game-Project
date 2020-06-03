using UnityEngine;

[System.Serializable]
public class PlayerWeapon
{
    public string WeaponName = "handgun";  //change settings for handgun
    public int damage1 = 25;
    public int range1 = 300;
    public int ammo1 = 11;

    public string WeaponName2 = "Automatic Pistol";    //change settings for Auto Pistol
    public int damage2 = 13;
    public int range2 = 250;
    public int ammo2 = 16;
    public float fireRate = 7f;
}
