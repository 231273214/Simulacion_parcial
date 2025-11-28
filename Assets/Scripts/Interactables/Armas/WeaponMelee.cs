using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponMelee : MonoBehaviour
{
    private Weapon weapon;
    private PlayerController controls;
    private bool canAttack = true;
    private Transform player;

    public void SetWeapon(Weapon w, Transform playerTransform)
    {
        // Verificar que el arma sea de tipo melee
        if (w.type != WeaponType.Melee)
        {
            Debug.LogWarning($"WeaponMelee intentó equipar un arma {w.type}: {w.name}");
            Destroy(gameObject);
            return;
        }

        weapon = w;
        player = playerTransform;
        controls = new PlayerController();
        controls.Enable();
    }

    void Update()
    {
        // Puedes agregar lógica visual aquí (animaciones, etc.)
    }

    public void Attack()
    {
        if (weapon == null || !canAttack) return;

        Debug.Log($"Atacando con {weapon.name} - Daño: {weapon.damage}");

        PerformMeleeAttack();
        StartCoroutine(AttackCooldown());
    }

    private void PerformMeleeAttack()
    {
        // Raycast para detectar enemigos en el rango melee
        RaycastHit hit;
        Vector3 attackDirection = player.forward;

        // Usar aim direction si está disponible
        Vector2 aimDir = Vector2.zero;
        if (controls != null)
        {
            aimDir = controls.Player.AimDirection.ReadValue<Vector2>();
        }

        if (aimDir.magnitude > 0.1f)
        {
            attackDirection = new Vector3(aimDir.x, 0, aimDir.y).normalized;
        }

        // Debug visual del ataque
        Debug.DrawRay(player.position + Vector3.up, attackDirection * weapon.meleeRange, Color.red, 1f);

        if (Physics.Raycast(player.position + Vector3.up, attackDirection, out hit, weapon.meleeRange))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                // Aplicar daño al enemigo
                ZombieHealth enemyHealth = hit.collider.GetComponent<ZombieHealth>();
                if (enemyHealth != null)
                {
                    enemyHealth.TakeDamage(weapon.damage);
                    Debug.Log($"Golpe a enemigo: {hit.collider.name} - Daño: {weapon.damage}");
                }
            }
        }

        // Aquí podrías agregar:
        // - Animaciones
        // - Efectos de sonido
        // - Partículas
        // - Feedback visual
    }

    private System.Collections.IEnumerator AttackCooldown()
    {
        canAttack = false;
        yield return new WaitForSeconds(weapon.meleeCooldown);
        canAttack = true;
    }

    void OnDestroy()
    {
        controls?.Disable();
        controls?.Dispose();
    }
}
