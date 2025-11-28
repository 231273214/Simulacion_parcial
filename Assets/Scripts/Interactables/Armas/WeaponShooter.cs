using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponShooter : MonoBehaviour
{
    private Weapon weapon;
    private float cooldown = 0f;
    private Transform firePoint;
    private Vector2 aimDirection = Vector2.right;

    public void SetWeapon(Weapon w, Transform firePoint)
    {
        // NUEVO: Verificar que el arma sea de tipo ranged
        if (w.type != WeaponType.Ranged)
        {
            Debug.LogWarning($"WeaponShooter intentó equipar un arma {w.type}: {w.name}");
            Destroy(gameObject);
            return;
        }

        weapon = w;
        this.firePoint = firePoint;
        cooldown = 0f;
    }

    public void UpdateAim(Vector2 direction)
    {
        if (direction.sqrMagnitude > 0.01f)
        {
            aimDirection = direction.normalized;
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            firePoint.rotation = Quaternion.Euler(0, 0, angle);
        }
        else
        {
            // Apuntar con mouse si no hay input de joystick
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            aimDirection = (mousePos - firePoint.position);
            float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
            firePoint.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    void Update()
    {
        if (weapon == null) return;
        cooldown -= Time.deltaTime;
    }

    public void Shoot()
    {
        if (weapon == null || cooldown > 0f) return;

        // NUEVO: Verificar que tenga prefab de proyectil
        if (weapon.projectilePrefab == null)
        {
            Debug.LogError($"No hay projectilePrefab asignado para el arma: {weapon.name}");
            return;
        }

        GameObject proj = Instantiate(weapon.projectilePrefab, firePoint.position, firePoint.rotation);
        Bullet bullet = proj.GetComponent<Bullet>();
        if (bullet != null)
        {
            bullet.damage = weapon.damage;
            bullet.speed = weapon.projectileSpeed;
        }

        cooldown = weapon.fireRate;
    }
}
