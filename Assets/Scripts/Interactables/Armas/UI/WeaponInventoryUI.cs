using UnityEngine;

public class WeaponInventoryUI : MonoBehaviour
{
    [Header("Slots")]
    public WeaponSlotUI slot1;
    public WeaponSlotUI slot2;
    public WeaponSlotUI slot3;
    public WeaponSlotUI slot4;

    [Header("Inventario Jugador")]
    public WeaponInventory playerInventory;

    private WeaponSlotUI[] slots;

    void Start()
    {
        slots = new WeaponSlotUI[] { slot1, slot2, slot3, slot4 };

        if (playerInventory == null)
            playerInventory = FindObjectOfType<WeaponInventory>();
    }

    void Update()
    {
        if (playerInventory == null) return;

        // Actualizar slots
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i] != null)
            {
                if (i < playerInventory.weapons.Count)
                {
                    // Mostrar arma
                    slots[i].ShowWeapon(GetWeaponIcon(playerInventory.weapons[i]));
                }
                else
                {
                    // Slot vacío
                    slots[i].ClearSlot();
                }
            }
        }
    }

    private Sprite GetWeaponIcon(Weapon weapon)
    {
        // Si tienes iconos, los cargas aquí
        // Por ahora null (solo limpia/llena el slot)
        return null;
    }
}