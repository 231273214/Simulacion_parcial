using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [Header("Stats")]
    public int maxHealth = 100;
    public int currentHealth;

    [Header("UI")]
    public Slider healthBar;

    public Animator animator;
    public bool isDead = false;

    private bool deathAnimationPlayed = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int amount)
    {
        if (isDead) return;

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();

        // Efecto de daño visual/sonoro aquí
        Debug.Log($"Player recibió {amount} de daño. Vida: {currentHealth}/{maxHealth}");

        if (currentHealth <= 0 && !isDead)
        {
            Die();
        }
    }

    void UpdateUI()
    {
        if (healthBar != null)
            healthBar.value = (float)currentHealth / maxHealth;
    }

    void Die()
    {
        isDead = true;

        if (!deathAnimationPlayed)
        {
            animator.SetTrigger("isDead");
            deathAnimationPlayed = true;

            Debug.Log("Player Muerto - Animación reproducida");
        }

        // Desactivar movimiento y otras habilidades
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = false;

        WeaponShooter weapons = GetComponent<WeaponShooter>();
        if (weapons != null)
            weapons.enabled = false;

        // Desactivar collider
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = false;
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateUI();
        Debug.Log($"Player curado: +{amount} de vida");
    }

    // Método para respawn
    public void Respawn()
    {
        isDead = false;
        deathAnimationPlayed = false;
        currentHealth = maxHealth;
        UpdateUI();

        // Reactivar componentes
        PlayerMovement movement = GetComponent<PlayerMovement>();
        if (movement != null)
            movement.enabled = true;

        WeaponShooter weapons = GetComponent<WeaponShooter>();
        if (weapons != null)
            weapons.enabled = true;

        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
            collider.enabled = true;

        // Resetear animación
        animator.ResetTrigger("isDead");
        animator.Play("Idle");

        Debug.Log("Player respawneado");
    }
}