using UnityEngine;

public class WeaponPickup : MonoBehaviour, IInteractable
{
    public Weapon weapon; // asignado en código o prefab

    public void Interact(GameObject player)
    {
        WeaponInventory inventory = player.GetComponent<WeaponInventory>();
        if (inventory != null)
        {
            inventory.AddWeapon(weapon);
            Destroy(gameObject);
        }
    }

    public string GetInteractText() => $"Recoger {weapon.name}";
}




