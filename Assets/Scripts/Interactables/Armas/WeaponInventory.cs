using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WeaponInventory : MonoBehaviour
{
    public List<WeaponData> weapons = new List<WeaponData>();
    public int currentWeaponIndex = 0;

    [Header("UI")]
    public Image[] weaponSlots;
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.white;
    public Text ammoText;
    public Text weaponNameText;
    public SpriteRenderer weaponSpriteRenderer;

    public WeaponShooter shooterPrefab;
    private WeaponShooter currentShooter;

    public WeaponData CurrentWeapon => weapons.Count > 0 ? weapons[currentWeaponIndex] : null;
    public WeaponShooter CurrentShooter => currentShooter;

    void Start()
    {
        if (weapons.Count > 0)
            EquipWeapon(0);
    }

    public void AddWeapon(WeaponData newWeapon)
    {
        if (weapons.Exists(w => w.type == newWeapon.type)) return;
        weapons.Add(newWeapon);
        UpdateInventoryUI();

        if (weapons.Count == 1)
            EquipWeapon(0);
    }

    public void EquipWeapon(int index)
    {
        if (weapons.Count == 0) return;
        index = Mathf.Clamp(index, 0, weapons.Count - 1);
        currentWeaponIndex = index;

        if (currentShooter != null) Destroy(currentShooter.gameObject);

        currentShooter = Instantiate(shooterPrefab, transform.position, Quaternion.identity, transform);
        currentShooter.SetWeaponData(CurrentWeapon);

        UpdateInventoryUI();
    }

    public void EquipNextWeapon()
    {
        if (weapons.Count == 0) return;
        EquipWeapon((currentWeaponIndex + 1) % weapons.Count);
    }

    public void EquipPreviousWeapon()
    {
        if (weapons.Count == 0) return;
        int newIndex = currentWeaponIndex - 1;
        if (newIndex < 0) newIndex = weapons.Count - 1;
        EquipWeapon(newIndex);
    }

    void UpdateInventoryUI()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (i < weapons.Count)
            {
                weaponSlots[i].sprite = weapons[i].weaponUISprite;
                weaponSlots[i].color = i == currentWeaponIndex ? selectedColor : normalColor;
            }
            else
            {
                weaponSlots[i].sprite = null;
                weaponSlots[i].color = Color.gray;
            }
        }

        if (CurrentWeapon != null)
        {
            ammoText.text = "?"; // Munición infinita
            weaponNameText.text = CurrentWeapon.name;
            if (weaponSpriteRenderer != null)
                weaponSpriteRenderer.sprite = CurrentWeapon.weaponSprite;
        }
    }
}