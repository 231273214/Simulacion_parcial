using UnityEngine;
using UnityEngine.UI;

public class WeaponSlotUI : MonoBehaviour
{
    public Image weaponIcon; // Solo esto

    public void ShowWeapon(Sprite icon)
    {
        if (weaponIcon != null)
        {
            weaponIcon.sprite = icon;
            weaponIcon.color = Color.white;
        }
    }

    public void ClearSlot()
    {
        if (weaponIcon != null)
        {
            weaponIcon.sprite = null;
            weaponIcon.color = new Color(1, 1, 1, 0.3f);
        }
    }
}
