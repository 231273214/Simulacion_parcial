using UnityEngine;

[System.Serializable]
public class Weapon
{
    public string name;
    public WeaponType type; 
    public GameObject projectilePrefab;
    public float fireRate;
    public float projectileSpeed;
    public int damage;

    // Campos para armas cuerpo a cuerpo
    public float meleeRange = 2f;
    public float meleeCooldown = 0.5f;

    public Weapon(string name, WeaponType type, GameObject prefab, float fireRate, float projSpeed, int damage)
    {
        this.name = name;
        this.type = type; 
        this.projectilePrefab = prefab;
        this.fireRate = fireRate;
        this.projectileSpeed = projSpeed;
        this.damage = damage;

        // Valores por defecto para melee
        this.meleeRange = 2f;
        this.meleeCooldown = 0.5f;
    }

    public Weapon(string name, WeaponType type, int damage, float meleeRange, float meleeCooldown)
    {
        this.name = name;
        this.type = type;
        this.damage = damage;
        this.meleeRange = meleeRange;
        this.meleeCooldown = meleeCooldown;

        // Para melee, estos no se usan
        this.projectilePrefab = null;
        this.fireRate = 0f;
        this.projectileSpeed = 0f;
    }
}

// Enum para tipos de arma
public enum WeaponType
{
    Ranged,
    Melee
}