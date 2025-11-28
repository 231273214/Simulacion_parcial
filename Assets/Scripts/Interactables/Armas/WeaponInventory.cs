using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System;

public class WeaponInventory : MonoBehaviour
{
    public List<Weapon> weapons = new List<Weapon>();
    public int currentWeaponIndex = 0;
    private WeaponShooter currentShooter;
    private WeaponMelee currentMelee; // NUEVO: Para armas cuerpo a cuerpo

    public WeaponShooter shooterPrefab;
    public Transform firePoint;

    private PlayerController controls;
    private bool canAttack = true; // NUEVO: Cooldown para melee

    //Eventos para la UI
    public event Action<Weapon> OnWeaponAdded;
    public event Action<Weapon> OnWeaponRemoved;
    public event Action<int> OnWeaponChanged;

    void Awake()
    {
        controls = new PlayerController();
        controls.Player.NextWeapon.performed += ctx => EquipNextWeapon();
        controls.Player.PreviousWeapon.performed += ctx => EquipPreviousWeapon();
        controls.Player.Shoot.performed += ctx => UseCurrentWeapon(); 
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    void Update()
    {
        // Cambiar armas con teclas 1-4
        if (Keyboard.current.digit1Key.wasPressedThisFrame) EquipWeapon(0);
        if (Keyboard.current.digit2Key.wasPressedThisFrame) EquipWeapon(1);
        if (Keyboard.current.digit3Key.wasPressedThisFrame) EquipWeapon(2);
        if (Keyboard.current.digit4Key.wasPressedThisFrame) EquipWeapon(3);

        // Actualizar dirección del disparo con joystick derecho (solo para ranged)
        Vector2 aimDir = controls.Player.AimDirection.ReadValue<Vector2>();
        if (currentShooter != null) currentShooter.UpdateAim(aimDir);
    }

    public Weapon CurrentWeapon => weapons.Count > 0 ? weapons[currentWeaponIndex] : null;

    public void AddWeapon(Weapon weapon)
    {
        if (weapons.Contains(weapon)) return;
        weapons.Add(weapon);
        EquipWeapon(weapons.Count - 1);

        OnWeaponAdded?.Invoke(weapon);
    }

    public void EquipWeapon(int index)
    {
        if (weapons.Count == 0) return;
        if (index < 0 || index >= weapons.Count) return;

        currentWeaponIndex = index;

        // Limpiar armas anteriores
        if (currentShooter != null)
        {
            Destroy(currentShooter.gameObject);
            currentShooter = null;
        }
        if (currentMelee != null)
        {
            Destroy(currentMelee.gameObject);
            currentMelee = null;
        }

        // Equipar según el tipo
        if (CurrentWeapon.type == WeaponType.Ranged)
        {
            currentShooter = Instantiate(shooterPrefab, firePoint.position, Quaternion.identity, transform);
            currentShooter.SetWeapon(CurrentWeapon, firePoint);
        }
        else if (CurrentWeapon.type == WeaponType.Melee)
        {
            // Crear componente melee
            GameObject meleeObj = new GameObject($"Melee_{CurrentWeapon.name}");
            meleeObj.transform.SetParent(transform);
            currentMelee = meleeObj.AddComponent<WeaponMelee>();
            currentMelee.SetWeapon(CurrentWeapon, transform);
        }
        OnWeaponChanged?.Invoke(index);
    }

    // Método unificado para usar cualquier arma
    private void UseCurrentWeapon()
    {
        if (CurrentWeapon == null) return;

        switch (CurrentWeapon.type)
        {
            case WeaponType.Ranged:
                currentShooter?.Shoot();
                break;
            case WeaponType.Melee:
                if (canAttack)
                {
                    PerformMeleeAttack();
                    StartCoroutine(MeleeCooldown());
                }
                break;
        }
    }

    // NUEVO: Lógica de ataque cuerpo a cuerpo
    private void PerformMeleeAttack()
    {
        Debug.Log($"Atacando con {CurrentWeapon.name} - Daño: {CurrentWeapon.damage}");

        // Raycast para detectar enemigos en el rango melee
        RaycastHit hit;
        Vector3 attackDirection = transform.forward;

        // Si hay aim direction, usar esa dirección
        Vector2 aimDir = controls.Player.AimDirection.ReadValue<Vector2>();
        if (aimDir.magnitude > 0.1f)
        {
            attackDirection = new Vector3(aimDir.x, 0, aimDir.y).normalized;
        }

        if (Physics.Raycast(transform.position + Vector3.up, attackDirection, out hit, CurrentWeapon.meleeRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                // Aplicar daño al enemigo
                ZombieHealth enemyHealth = hit.collider.GetComponent<ZombieHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(CurrentWeapon.damage);
                    Debug.Log($"Golpe a enemigo: {hit.collider.name}");
                }
            }
        }

        // Aquí podrías agregar animaciones, efectos, etc.
    }

    // NUEVO: Corrutina para cooldown melee
    private System.Collections.IEnumerator MeleeCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(CurrentWeapon.meleeCooldown);
        canAttack = true;
    }

    public void EquipNextWeapon()
    {
        if (weapons.Count == 0) return;
        EquipWeapon((currentWeaponIndex + 1) % weapons.Count);
    }

    public void EquipPreviousWeapon()
    {
        if (weapons.Count == 0) return;
        int newIndex = currentWeaponIndex - 1;
        if (newIndex < 0) newIndex = weapons.Count - 1;
        EquipWeapon(newIndex);
    }
}