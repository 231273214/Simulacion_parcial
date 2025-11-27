using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponInputHandler : MonoBehaviour
{
    public WeaponShooter shooter;
    private PlayerController controls;

    private Vector2 aimDirection;

    void Awake()
    {
        controls = new PlayerController();

        // DISPARO (gatillo derecho)
        controls.Player.Shoot.performed += ctx =>
        {
            if (shooter != null)
                shooter.HandleShootInput();
        };

        // RECARGA
        //controls.Player.Reload.performed += ctx =>
        //{
           // if (shooter != null)
                //shooter.HandleReloadInput();
        //};

        // Cambiar arma siguiente
        controls.Player.NextWeapon.performed += ctx =>
        {
            if (shooter != null)
                shooter.HandleNextWeaponInput();
        };

        // Cambiar arma anterior
        controls.Player.PreviousWeapon.performed += ctx =>
        {
            if (shooter != null)
                shooter.HandlePreviousWeaponInput();
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

