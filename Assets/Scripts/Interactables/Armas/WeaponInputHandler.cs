using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponInputHandler : MonoBehaviour
{
    private PlayerController controls;
    private WeaponInventory weaponInventory;

    private Vector2 aimDirection;

    void Awake()
    {
        controls = new PlayerController();
        weaponInventory = GetComponent<WeaponInventory>();

        // Disparo
        controls.Player.Shoot.performed += ctx =>
        {
            if (weaponInventory.CurrentShooter != null)
                weaponInventory.CurrentShooter.HandleShootInput();
        };

        // Cambiar arma siguiente
        controls.Player.NextWeapon.performed += ctx =>
        {
            weaponInventory.EquipNextWeapon();
        };

        // Cambiar arma anterior
        controls.Player.PreviousWeapon.performed += ctx =>
        {
            weaponInventory.EquipPreviousWeapon();
        };

        // Dirección de apuntado (joystick derecho)
        controls.Player.AimDirection.performed += ctx =>
        {
            aimDirection = ctx.ReadValue<Vector2>();
        };
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    public Vector2 GetAimDirection() => aimDirection;
}


