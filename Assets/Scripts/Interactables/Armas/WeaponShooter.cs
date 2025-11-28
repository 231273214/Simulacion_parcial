using UnityEngine;
using System.Collections;

public class WeaponShooter : MonoBehaviour
{
    public Transform firePoint;
    public AudioSource audioSource;
    private WeaponData weaponData;

    public void SetWeaponData(WeaponData data)
    {
        weaponData = data;
    }

    public void TryShoot()
    {
        if (weaponData == null) return;

        Shoot();
    }

    void Shoot()
    {
        if (weaponData.shootSound && audioSource)
            audioSource.PlayOneShot(weaponData.shootSound);

        switch (weaponData.type)
        {
            case WeaponType.Pistol: ShootProjectile(); break;
            case WeaponType.Shotgun: ShootShotgun(); break;
            case WeaponType.Sniper: ShootProjectile(); break;
            case WeaponType.Knife: MeleeAttack(); break;
        }
    }

    void ShootProjectile()
    {
        if (!weaponData.projectilePrefab) return;

        GameObject proj = Instantiate(weaponData.projectilePrefab, firePoint.position, Quaternion.identity);
        Vector2 dir = GetShootDirection();
        proj.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        Bullet b = proj.GetComponent<Bullet>();
        if (b != null)
        {
            b.damage = weaponData.damage;
            b.speed = weaponData.projectileSpeed;
        }
    }

    void ShootShotgun()
    {
        Vector2 baseDir = GetShootDirection();
        float[] angles = { 0, 15, -15, 30, -30 };
        foreach (float a in angles)
        {
            Vector2 dir = Quaternion.Euler(0, 0, a) * baseDir;
            GameObject proj = Instantiate(weaponData.projectilePrefab, firePoint.position, Quaternion.identity);
            proj.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
            Bullet b = proj.GetComponent<Bullet>();
            if (b != null)
            {
                b.damage = weaponData.damage;
                b.speed = weaponData.projectileSpeed;
            }
        }
    }

    void MeleeAttack()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(firePoint.position, weaponData.range);
        foreach (var t in targets)
            if (t.CompareTag("Zombie"))
                t.GetComponent<ZombieHealth>()?.TakeDamage(weaponData.damage);
    }

    Vector2 GetShootDirection()
    {
        // Prioriza joystick
        if (WeaponInputHandler.Instance != null)
        {
            Vector2 dir = WeaponInputHandler.Instance.GetAimDirection();
            if (dir.magnitude > 0.2f) return dir.normalized;
        }

        // Mouse
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(UnityEngine.InputSystem.Mouse.current.position.ReadValue());
        return (mousePos - (Vector2)firePoint.position).normalized;
    }
}


