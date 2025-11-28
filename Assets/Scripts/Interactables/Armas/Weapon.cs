using UnityEngine;

[System.Serializable]
public class Weapon
{
    public string name;
    public GameObject projectilePrefab;
    public float fireRate;
    public float projectileSpeed;
    public int damage;

    public Weapon(string name, GameObject prefab, float fireRate, float projSpeed, int damage)
    {
        this.name = name;
        this.projectilePrefab = prefab;
        this.fireRate = fireRate;
        this.projectileSpeed = projSpeed;
        this.damage = damage;
    }
}