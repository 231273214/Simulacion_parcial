using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponInputHandler : MonoBehaviour
{
    public static WeaponInputHandler Instance;
    private PlayerController controls;
    private Vector2 aimDir;

    void Awake()
    {
        if (Instance == null) Instance = this;

        controls = new PlayerController();

        controls.Player.AimDirection.performed += ctx => aimDir = ctx.ReadValue<Vector2>();
        controls.Player.Shoot.performed += ctx =>
        {
            WeaponInventory inv = GetComponent<WeaponInventory>();
            inv?.CurrentShooter?.TryShoot();
        };
        controls.Player.NextWeapon.performed += ctx =>
        {
            WeaponInventory inv = GetComponent<WeaponInventory>();
            inv?.EquipNextWeapon();
        };
        controls.Player.PreviousWeapon.performed += ctx =>
        {
            WeaponInventory inv = GetComponent<WeaponInventory>();
            inv?.EquipPreviousWeapon();
        };
    }

    void OnEnable() => controls.Enable();
    void OnDisable() => controls.Disable();

    public Vector2 GetAimDirection() => aimDir;
}



