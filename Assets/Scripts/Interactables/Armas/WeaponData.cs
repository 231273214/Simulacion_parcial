using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Weapons/WeaponData")]
public class WeaponData : ScriptableObject
{
    public string name;
    public WeaponType type;
    public int damage;
    public float cooldown;
    public float range;
    public string description;

    public Sprite weaponSprite;
    public Sprite weaponUISprite;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public GameObject projectilePrefab;
    public float projectileSpeed = 12f; // velocidad del proyectil
}

public enum WeaponType
{
    Pistol,
    Shotgun,
    Sniper,
    Knife
}

