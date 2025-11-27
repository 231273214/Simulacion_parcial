using UnityEngine;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    public WeaponType weaponType;
    public int ammoAmount = 0;

    [Header("Visual")]
    public Sprite weaponSprite;
    public Sprite weaponUISprite;

    [Header("Sonidos")]
    public AudioClip shootSound;
    public AudioClip reloadSound;

    [Header("Prefab de Bala")]
    public GameObject projectilePrefab;

    private WeaponInventory playerInventory;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Buscamos el WeaponInventory aunque esté en un hijo
            playerInventory = other.GetComponentInChildren<WeaponInventory>();

            if (playerInventory != null)
                Debug.Log("WeaponInventory detectado en jugador.");
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInventory = null;
        }
    }

    // Método de interacción
    public void Interact(GameObject interactor)
    {
        if (playerInventory == null) return;

        // Añadir arma
        playerInventory.AddWeapon(
            weaponType,
            weaponSprite,
            weaponUISprite,
            shootSound,
            reloadSound,
            projectilePrefab
        );

        // Añadir munición si viene incluida
        if (ammoAmount > 0)
            playerInventory.AddAmmo(weaponType, ammoAmount);

        Debug.Log($"Arma {weaponType} recogida.");

        Destroy(gameObject);
    }

    public string GetInteractText()
    {
        string deviceLabel = "E";

#if ENABLE_INPUT_SYSTEM
        var gp = UnityEngine.InputSystem.Gamepad.current;
        if (gp != null)
        {
            string name = gp.displayName.ToLower();
            if (name.Contains("xbox")) deviceLabel = "A";
            else if (name.Contains("dualshock") || name.Contains("dualsense") || name.Contains("ps")) deviceLabel = "X";
            else deviceLabel = "A";
        }
#endif

        return $"Presiona <b>{deviceLabel}</b> para recoger";
    }
}



