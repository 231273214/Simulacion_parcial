using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    public WeaponType weaponType;

    [Header("Visual")]
    public Sprite weaponSprite;
    public Sprite weaponUISprite;

    [Header("Sonidos")]
    public AudioClip shootSound;
    public AudioClip reloadSound;

    [Header("Prefab de Bala")]
    public GameObject projectilePrefab;

    private WeaponInventory playerInventory;

    public void Interact(GameObject interactor)
    {
        if (playerInventory == null)
            playerInventory = interactor.GetComponentInChildren<WeaponInventory>();

        if (playerInventory == null)
        {
            Debug.LogError("El jugador no tiene WeaponInventory");
            return;
        }

        // Crear el arma y añadirla al inventario
        playerInventory.AddWeapon(
            weaponType,
            weaponSprite,
            weaponUISprite,
            shootSound,
            reloadSound,
            projectilePrefab
        );

        Debug.Log($"Arma {weaponType} recogida.");

        // Destruir objeto del mundo
        Destroy(gameObject);
    }

    public string GetInteractText()
    {
        string deviceLabel = "E";

#if ENABLE_INPUT_SYSTEM
        if (Gamepad.current != null)
            deviceLabel = "A"; // botón principal del control
#endif
        return $"Presiona <b>{deviceLabel}</b> para recoger";
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            playerInventory = other.GetComponentInChildren<WeaponInventory>();
    }
}
