using UnityEngine;
using System.Collections;

public class ZombieHealth : MonoBehaviour
{
    [Header("Configuración de Vida")]
    public int maxHealth = 50;
    public int currentHealth;
    public bool isDead = false;

    [Header("Efectos Visuales")]
    public GameObject deathEffect;
    public GameObject bloodEffect;
    public SpriteRenderer zombieSprite;
    public Color damageColor = Color.red;
    public float flashDuration = 0.1f;

    [Header("Sonidos")]
    public AudioClip deathSound;
    public AudioClip damageSound;
    public AudioClip spawnSound;

    [Header("Loot")]
    public GameObject[] lootDrops;
    public float lootDropChance = 0.3f;

    [Header("Animations")]
    public Animator animator;

    private AudioSource audioSource;
    private ZombieAI zombieAI;
    private Color originalColor;
    private Coroutine flashCoroutine;

    void Start()
    {
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        zombieAI = GetComponent<ZombieAI>();
        zombieSprite = GetComponent<SpriteRenderer>();

        if (zombieSprite != null)
        {
            originalColor = zombieSprite.color;
        }

        if (audioSource != null && spawnSound != null)
        {
            audioSource.PlayOneShot(spawnSound);
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);

        Debug.Log($"Zombie recibió {damage} de daño. Vida restante: {currentHealth}/{maxHealth}");

        // Efecto visual de daño
        if (flashCoroutine != null)
            StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashDamage());

        // Sonido de daño
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        // Efecto de sangre
        if (bloodEffect != null)
        {
            Instantiate(bloodEffect, transform.position, Quaternion.identity);
        }

        // Verificar muerte
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashDamage()
    {
        if (zombieSprite != null)
        {
            zombieSprite.color = damageColor;
            yield return new WaitForSeconds(flashDuration);
            zombieSprite.color = originalColor;
        }
    }

    void Die()
    {
        if (isDead) return;

        isDead = true;
        Debug.Log("Zombie eliminado!");

        Debug.Log($"Animator: {animator != null}");
        if (animator != null)
        {
            Debug.Log($"Animator enabled: {animator.enabled}");
            // ... resto del debug
        }

        // PRIMERO la animación
        if (animator != null)
        {
            animator.SetBool("isDead", true);
            Debug.Log("Trigger Die activado");
        }

        // LUEGO sonido
        if (audioSource != null && deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

        
        StartCoroutine(DisableComponentsAfterDelay());

        // Misiones y loot
        if (MissionManager.Instance != null)
        {
            MissionManager.Instance.UpdateMissionProgress("KILL_10_ZOMBIES", 1);
            MissionManager.Instance.UpdateMissionProgress("KILL_25_ZOMBIES", 1);
        }
        DropLoot();

        // Destruir después de la animación
        StartCoroutine(DestroyAfterAnimation());
    }

    IEnumerator DestroyAfterAnimation()
    {
        // Esperar a que termine la animación de muerte
        if (animator != null)
        {
            // Obtener la duración de la animación actual
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            float animationLength = stateInfo.length;

            // Esperar la duración completa de la animación
            yield return new WaitForSeconds(animationLength);
        }
        else
        {
            yield return new WaitForSeconds(1f);
        }

        Destroy(gameObject);
    }

    IEnumerator DisableComponentsAfterDelay()
    {
        // Esperar un frame para que la animación empiece
        yield return new WaitForSeconds(0.1f);

        // Desactivar AI
        if (zombieAI != null)
            zombieAI.enabled = false;

        // Desactivar colliders
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D collider in colliders)
        {
            collider.enabled = false;
        }
    }

    void DropLoot()
    {
        if (lootDrops.Length > 0 && Random.value <= lootDropChance)
        {
            GameObject loot = lootDrops[Random.Range(0, lootDrops.Length)];
            if (loot != null)
            {
                Instantiate(loot, transform.position, Quaternion.identity);
                Debug.Log("Zombie soltó loot!");
            }
        }
    }

    // Método para curación (por si acaso)
    public void Heal(int healAmount)
    {
        if (isDead) return;

        currentHealth += healAmount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        Debug.Log($"Zombie curado. Vida: {currentHealth}/{maxHealth}");
    }
    
}
