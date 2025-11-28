using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponInventory : MonoBehaviour
{
    public List<Weapon> weapons = new List<Weapon>();
    public int currentWeaponIndex = 0;
    private WeaponShooter currentShooter;

    public WeaponShooter shooterPrefab;
    public Transform firePoint;

    private PlayerController controls;

    void Awake()
    {
        controls = new PlayerController();
        controls.Player.NextWeapon.performed += ctx => EquipNextWeapon();
        controls.Player.PreviousWeapon.performed += ctx => EquipPreviousWeapon();
        controls.Player.Shoot.performed += ctx => currentShooter?.Shoot();
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

        // Actualizar dirección del disparo con joystick derecho
        Vector2 aimDir = controls.Player.AimDirection.ReadValue<Vector2>();
        if (currentShooter != null) currentShooter.UpdateAim(aimDir);
    }

    public Weapon CurrentWeapon => weapons.Count > 0 ? weapons[currentWeaponIndex] : null;

    public void AddWeapon(Weapon weapon)
    {
        if (weapons.Contains(weapon)) return;
        weapons.Add(weapon);
        EquipWeapon(weapons.Count - 1);
    }

    public void EquipWeapon(int index)
    {
        if (weapons.Count == 0) return;
        if (index < 0 || index >= weapons.Count) return;

        currentWeaponIndex = index;

        if (currentShooter != null) Destroy(currentShooter.gameObject);

        currentShooter = Instantiate(shooterPrefab, firePoint.position, Quaternion.identity, transform);
        currentShooter.SetWeapon(CurrentWeapon, firePoint);
    }

    public void EquipNextWeapon()
    {
        EquipWeapon((currentWeaponIndex + 1) % weapons.Count);
    }

    public void EquipPreviousWeapon()
    {
        int newIndex = currentWeaponIndex - 1;
        if (newIndex < 0) newIndex = weapons.Count - 1;
        EquipWeapon(newIndex);
    }
}


