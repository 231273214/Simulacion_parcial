using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;

public class WeaponShooter : MonoBehaviour
{
    [Header("Referencias")]
    public Transform firePoint;
    public AudioSource audioSource;
    public WeaponInventory weaponInventory;
    public WeaponInputHandler inputHandler;

    private float lastFireTime;
    private bool isReloading = false;

    void Update()
    {
        // Si usa mouse, deja que apunte normal
        RotateToAimDirection();
    }

    // ===============================
    //    ENTRADAS DEL JUGADOR
    // ===============================
    public void HandleShootInput()
    {
        TryShoot();
    }

    public void HandleReloadInput()
    {
        TryReload();
    }

    public void HandleNextWeaponInput()
    {
        weaponInventory.EquipNextWeapon();
    }

    public void HandlePreviousWeaponInput()
    {
        weaponInventory.EquipPreviousWeapon();
    }

    // ===============================
    //            DISPARAR
    // ===============================
    public void TryShoot()
    {
        if (weaponInventory == null || weaponInventory.CurrentWeapon == null) return;
        if (isReloading) return;

        if (Time.time >= lastFireTime + weaponInventory.CurrentWeapon.cooldown)
        {
            if (weaponInventory.CanShoot())
                Shoot();
            else
                Debug.Log("Sin balas");
        }
    }

    void Shoot()
    {
        lastFireTime = Time.time;
        weaponInventory.ConsumeAmmo();

        // Sonido
        if (audioSource && weaponInventory.CurrentWeapon.shootSound)
            audioSource.PlayOneShot(weaponInventory.CurrentWeapon.shootSound);

        // Tipo de arma
        switch (weaponInventory.CurrentWeapon.type)
        {
            case WeaponType.Pistol:
                ShootPistol();
                break;
            case WeaponType.Shotgun:
                ShootShotgun();
                break;
            case WeaponType.Sniper:
                ShootSniper();
                break;
            case WeaponType.Knife:
                ShootKnife();
                break;
        }
    }

    // ===============================
    //            DIRECCIÓN
    // ===============================
    Vector2 GetShootDirection()
    {
        // 1?? Dirección del joystick derecho
        Vector2 stickDir = inputHandler.GetAimDirection();
        if (stickDir.magnitude > 0.2f)
            return stickDir.normalized;

        // 2?? Dirección del mouse
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        return (mousePos - (Vector2)firePoint.position).normalized;
    }

    void RotateToAimDirection()
    {
        Vector2 dir = GetShootDirection();
        if (dir.sqrMagnitude <= 0.01f) return;

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        firePoint.rotation = Quaternion.Euler(0, 0, angle);
    }

    // ===============================
    //        LÓGICA DE BALAS
    // ===============================
    void ShootPistol()
    {
        CreateProjectile(GetShootDirection(), weaponInventory.CurrentWeapon.damage, 12f);
    }

    void ShootShotgun()
    {
        Vector2 baseDir = GetShootDirection();
        float[] angles = { 0, 15, -15, 30, -30 };

        foreach (float a in angles)
            CreateProjectile(Quaternion.Euler(0, 0, a) * baseDir,
                weaponInventory.CurrentWeapon.damage, 8f);
    }

    void ShootSniper()
    {
        CreateProjectile(GetShootDirection(), weaponInventory.CurrentWeapon.damage, 20f);
    }

    void ShootKnife()
    {
        Collider2D[] hitTargets =
            Physics2D.OverlapCircleAll(firePoint.position, weaponInventory.CurrentWeapon.range);

        foreach (Collider2D t in hitTargets)
            if (t.CompareTag("Zombie"))
                t.GetComponent<ZombieHealth>()?.TakeDamage(weaponInventory.CurrentWeapon.damage);
    }

    void CreateProjectile(Vector2 direction, int damage, float speed)
    {
        if (!weaponInventory.CurrentWeapon.projectilePrefab) return;

        GameObject projectile =
            Instantiate(weaponInventory.CurrentWeapon.projectilePrefab, firePoint.position, Quaternion.identity);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        projectile.transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Bullet b = projectile.GetComponent<Bullet>();
        if (b != null)
        {
            b.damage = damage;
            b.speed = speed;
        }
    }

    // ===============================
    //         RECARGAR
    // ===============================
    public void TryReload()
    {
        if (weaponInventory.weapons.Count == 0 || weaponInventory.CurrentWeapon == null) return;
        if (isReloading) return;

        if (weaponInventory.CurrentWeapon.type == WeaponType.Knife) return;

        if (weaponInventory.CurrentWeapon.currentAmmo >= weaponInventory.CurrentWeapon.maxAmmo) return;

        StartCoroutine(ReloadWeapon());
    }

    IEnumerator ReloadWeapon()
    {
        isReloading = true;

        if (audioSource && weaponInventory.CurrentWeapon.reloadSound)
            audioSource.PlayOneShot(weaponInventory.CurrentWeapon.reloadSound);

        yield return new WaitForSeconds(weaponInventory.CurrentWeapon.cooldown);

        weaponInventory.CurrentWeapon.currentAmmo = weaponInventory.CurrentWeapon.maxAmmo;
        weaponInventory.UpdateInventoryUI();
        isReloading = false;
    }
}
