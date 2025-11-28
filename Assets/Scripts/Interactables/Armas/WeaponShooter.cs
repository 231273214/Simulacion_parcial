using UnityEngine;
using System.Collections;

public class WeaponShooter : MonoBehaviour
{
    public Transform firePoint;
    public AudioSource audioSource;

    [HideInInspector] public WeaponData weaponData;

    private float lastFireTime;
    private bool isReloading = false;

    public void SetWeaponData(WeaponData data)
    {
        weaponData = data;
    }

    public void HandleShootInput()
    {
        TryShoot();
    }

    void TryShoot()
    {
        if (weaponData == null) return;
        if (isReloading) return;

        if (Time.time >= lastFireTime + weaponData.cooldown)
        {
            Shoot();
            lastFireTime = Time.time;
        }
    }

    void Shoot()
    {
        // Sonido
        if (audioSource && weaponData.shootSound)
            audioSource.PlayOneShot(weaponData.shootSound);

        // Proyectil
        if (weaponData.projectilePrefab)
        {
            GameObject projectile = Instantiate(weaponData.projectilePrefab, firePoint.position, firePoint.rotation);
            Bullet b = projectile.GetComponent<Bullet>();
            if (b != null)
            {
                b.damage = weaponData.damage;
                b.speed = weaponData.projectileSpeed;
            }
        }
    }

    public void RotateToDirection(Vector2 direction)
    {
        if (direction.sqrMagnitude <= 0.01f) return;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);
    }
}

