using UnityEngine;

[System.Serializable]
public class WeaponData
{
    public string name;
    public WeaponType type;
    public int damage;
    public int maxAmmo;
    public int currentAmmo;
    public float cooldown;
    public float range;
    public GameObject projectilePrefab;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public Sprite weaponSprite;
    public Sprite weaponUISprite;
    [TextArea] public string description;
}

public enum WeaponType
{
    Pistol,
    Shotgun,
    Sniper,
    Knife
}
