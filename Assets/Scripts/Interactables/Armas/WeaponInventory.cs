using UnityEngine;
using System.Collections.Generic;

public class WeaponInventory : MonoBehaviour
{
    public List<WeaponData> weapons = new List<WeaponData>();
    public List<WeaponShooter> weaponObjects = new List<WeaponShooter>();
    public int currentWeaponIndex = 0;

    [Header("Plantillas")]
    public WeaponData[] weaponTemplates;

    public WeaponData CurrentWeapon => weapons.Count > 0 ? weapons[currentWeaponIndex] : null;
    public WeaponShooter CurrentShooter => weaponObjects.Count > 0 ? weaponObjects[currentWeaponIndex] : null;

    // =============================================================
    public void AddWeapon(
        WeaponType type,
        Sprite weaponSprite,
        Sprite weaponUISprite,
        AudioClip shootSound,
        AudioClip reloadSound,
        GameObject projectilePrefab)
    {
        // Permitir múltiples armas, no borrar las anteriores
        WeaponData template = CreateWeaponByType(type);
        if (template == null)
        {
            Debug.LogError("No existe plantilla para " + type);
            return;
        }

        template.weaponSprite = weaponSprite;
        template.weaponUISprite = weaponUISprite;
        template.shootSound = shootSound;
        template.reloadSound = reloadSound;
        template.projectilePrefab = projectilePrefab;

        // Munición infinita
        template.maxAmmo = -1;
        template.currentAmmo = 9999;

        // Añadir a inventario
        weapons.Add(template);

        GameObject shooterObj = new GameObject(type.ToString() + "_Shooter");
        shooterObj.transform.SetParent(this.transform);
        WeaponShooter shooter = shooterObj.AddComponent<WeaponShooter>();
        shooter.SetWeaponData(template);
        shooterObj.SetActive(false);
        weaponObjects.Add(shooter);

        // Si es la primera arma recogida, equiparla
        if (weapons.Count == 1)
            EquipWeapon(0);
    }

    public void EquipWeapon(int index)
    {
        if (weapons.Count == 0) return;
        index = Mathf.Clamp(index, 0, weapons.Count - 1);

        for (int i = 0; i < weaponObjects.Count; i++)
            weaponObjects[i].gameObject.SetActive(i == index);

        currentWeaponIndex = index;
    }

    public void EquipNextWeapon()
    {
        if (weapons.Count == 0) return;
        currentWeaponIndex = (currentWeaponIndex + 1) % weapons.Count;
        EquipWeapon(currentWeaponIndex);
    }

    public void EquipPreviousWeapon()
    {
        if (weapons.Count == 0) return;
        currentWeaponIndex--;
        if (currentWeaponIndex < 0) currentWeaponIndex = weapons.Count - 1;
        EquipWeapon(currentWeaponIndex);
    }

    private WeaponData CreateWeaponByType(WeaponType type)
    {
        foreach (var t in weaponTemplates)
            if (t.type == type)
                return Instantiate(t);

        return null;
    }
}

