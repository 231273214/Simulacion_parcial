using UnityEngine;


public class Weapon
{
    public WeaponType type;
    public Sprite icon;
    public Sprite uiIcon;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public GameObject projectilePrefab;

    public int damage = 10;
    public float cooldown = 0.3f;
    public float range = 1f;

    public int currentAmmo = 9999;   // INFINITA
    public int maxAmmo = 9999;

    public Weapon(WeaponType type, Sprite icon, Sprite uiIcon,
                  AudioClip shootSound, AudioClip reloadSound,
                  GameObject projectilePrefab)
    {
        this.type = type;
        this.icon = icon;
        this.uiIcon = uiIcon;
        this.shootSound = shootSound;
        this.reloadSound = reloadSound;
        this.projectilePrefab = projectilePrefab;

        this.currentAmmo = 9999;
        this.maxAmmo = 9999;
    }
}

