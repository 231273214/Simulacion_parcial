using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class WeaponInventory : MonoBehaviour
{
    [Header("Armas del jugador")]
    public List<WeaponData> weapons = new List<WeaponData>();
    public List<WeaponShooter> weaponObjects = new List<WeaponShooter>();
    public int currentWeaponIndex = 0;

    [Header("UI")]
    public GameObject inventoryPanel;
    public Image[] weaponSlots;
    public Color selectedColor = Color.yellow;
    public Color normalColor = Color.white;
    public Color emptyColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);
    public Text ammoText;
    public Text weaponNameText;
    public SpriteRenderer weaponSpriteRenderer;

    [Header("Plantillas")]
    public WeaponData[] weaponTemplates;

    private bool infiniteAmmo = false;

    public WeaponData CurrentWeapon => weapons.Count > 0 ? weapons[currentWeaponIndex] : null;
    public WeaponShooter CurrentShooter => weaponObjects.Count > 0 ? weaponObjects[currentWeaponIndex] : null;

    void Start()
    {
        UpdateInventoryUI();
        EquipWeapon(currentWeaponIndex);
    }

    // ==========================
    //      AGREGAR ARMA
    // ==========================

    public void AddWeapon(
        WeaponShooter shooterPrefab,
        WeaponType type,
        Sprite weaponSprite,
        Sprite weaponUISprite,
        AudioClip shootSound,
        AudioClip reloadSound,
        GameObject projectilePrefab)
    {
        // 1. Crear WeaponData basado en plantilla
        WeaponData data = CreateWeaponByType(type);
        data.weaponSprite = weaponSprite;
        data.weaponUISprite = weaponUISprite;
        data.shootSound = shootSound;
        data.reloadSound = reloadSound;
        data.projectilePrefab = projectilePrefab;

        weapons.Add(data);

        // 2. Instanciar y registrar Shooter
        WeaponShooter shooter = Instantiate(shooterPrefab, transform);
        shooter.gameObject.SetActive(false);
        weaponObjects.Add(shooter);

        // 3. Equipar si es la primera arma
        if (weapons.Count == 1)
            EquipWeapon(0);

        UpdateInventoryUI();
    }

    public bool CanShoot()
    {
        if (CurrentWeapon == null) return false;

        // Armas con munición infinita
        if (CurrentWeapon.maxAmmo == -1)
            return true;

        return CurrentWeapon.currentAmmo > 0;
    }

    public void ConsumeAmmo()
    {
        if (CurrentWeapon == null) return;

        // Munición infinita
        if (CurrentWeapon.maxAmmo == -1)
            return;

        CurrentWeapon.currentAmmo = Mathf.Max(0, CurrentWeapon.currentAmmo - 1);

        UpdateInventoryUI();
    }


    // ==========================
    //   CAMBIO DE ARMA ACTIVA
    // ==========================

    public void EquipWeapon(int index)
    {
        if (weapons.Count == 0) return;

        index = Mathf.Clamp(index, 0, weapons.Count - 1);

        // Apagar todos los shooters
        for (int i = 0; i < weaponObjects.Count; i++)
            weaponObjects[i].gameObject.SetActive(false);

        // Activar el shooter correcto
        weaponObjects[index].gameObject.SetActive(true);

        currentWeaponIndex = index;

        UpdateInventoryUI();
    }

    public void EquipNextWeapon()
    {
        if (weapons.Count <= 1) return;

        currentWeaponIndex++;
        if (currentWeaponIndex >= weapons.Count)
            currentWeaponIndex = 0;

        EquipWeapon(currentWeaponIndex);
    }

    public void EquipPreviousWeapon()
    {
        if (weapons.Count <= 1) return;

        currentWeaponIndex--;
        if (currentWeaponIndex < 0)
            currentWeaponIndex = weapons.Count - 1;

        EquipWeapon(currentWeaponIndex);
    }

    // ==========================
    //         UI
    // ==========================

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (i < weapons.Count)
            {
                weaponSlots[i].sprite = weapons[i].weaponUISprite;
                weaponSlots[i].color = i == currentWeaponIndex ? selectedColor : normalColor;

                if (weaponSlots[i].transform.childCount > 0)
                {
                    Text t = weaponSlots[i].transform.GetChild(0).GetComponent<Text>();
                    if (t != null) t.text = (i + 1).ToString();
                }
            }
            else
            {
                weaponSlots[i].sprite = null;
                weaponSlots[i].color = emptyColor;
            }
        }

        if (CurrentWeapon != null)
        {
            ammoText.text = GetAmmoText();
            weaponNameText.text = CurrentWeapon.name;
            if (weaponSpriteRenderer != null)
                weaponSpriteRenderer.sprite = CurrentWeapon.weaponSprite;
        }
    }

    public string GetAmmoText()
    {
        if (CurrentWeapon.maxAmmo == -1)
            return "?";

        return $"{CurrentWeapon.currentAmmo}/{CurrentWeapon.maxAmmo}";
    }

    // ==========================
    //   CREAR WEAPON DATA
    // ==========================

    private WeaponData CreateWeaponByType(WeaponType type)
    {
        foreach (var t in weaponTemplates)
        {
            if (t.type == type)
            {
                return new WeaponData
                {
                    name = t.name,
                    type = t.type,
                    damage = t.damage,
                    maxAmmo = t.maxAmmo,
                    currentAmmo = t.maxAmmo,
                    cooldown = t.cooldown,
                    range = t.range,
                    description = t.description
                };
            }
        }

        Debug.LogWarning("Plantilla no encontrada para " + type);
        return new WeaponData();
    }
}

