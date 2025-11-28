using UnityEngine;
using TMPro;

public class CurrentWeaponDisplay : MonoBehaviour
{
    public TextMeshProUGUI weaponText;
    public WeaponInventory playerInventory;

    void Start()
    {
        if (playerInventory == null)
            playerInventory = FindObjectOfType<WeaponInventory>();
    }

    void Update()
    {
        if (weaponText != null && playerInventory != null)
        {
            if (playerInventory.CurrentWeapon != null)
            {
                weaponText.text = playerInventory.CurrentWeapon.name;
            }
            else
            {
                weaponText.text = "Sin arma";
            }
        }
    }
}
