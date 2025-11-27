using UnityEngine;
using System.Collections;

public class ZombieAI : MonoBehaviour
{
    [Header("Stats")]
    public float speed = 1.5f;
    public float visionRange = 5f;
    public float attackRange = 1.2f;
    public float directionChangeInterval = 3f;

    [Header("Animations")]
    public Animator animator;

    private Rigidbody2D rb;
    private Transform player;
    private ZombieHealth zombieHealth;

    private Vector2 patrolDirection;
    private float directionTimer;
    private bool isChasing = false;
    private bool isAttacking = false;

    [Header("Combat")]
    public int damage = 10;
    public float attackCooldown = 1f;

    private bool canDamage = true;
    private PlayerHealth playerHealth;

    [Header("Sonidos")]
    public AudioClip idleSound;
    public AudioClip chaseSound;
    public AudioClip attackSound;
    public float minSoundDelay = 3f;
    public float maxSoundDelay = 8f;

    private AudioSource audioSource;
    private float soundTimer;
    private float nextSoundTime;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        playerHealth = player.GetComponent<PlayerHealth>();
        zombieHealth = GetComponent<ZombieHealth>();

        ChooseNewPatrolDirection();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.spatialBlend = 1f;
            audioSource.volume = 0.7f;
        }
        ResetSoundTimer();

    }

    void Update()
    {
        // No hacer nada si está muerto
        if (zombieHealth != null && zombieHealth.isDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // --- Cambiar de estado ---
        if (distance <= attackRange)
        {
            StartAttack();
        }
        else if (distance <= visionRange)
        {
            StartChasing();
        }
        else
        {
            Patrol();
        }
        HandleSounds();

        HandleFlip();

        if (playerHealth != null && playerHealth.isDead)
        {
            animator.SetBool("IsAttacking", false);
            return;
        }
    }

    void HandleSounds()
    {
        soundTimer += Time.deltaTime;
        if (soundTimer >= nextSoundTime)
        {
            PlayStateSound();
            ResetSoundTimer();
        }
    }

    void PlayStateSound()
    {
        if (audioSource == null || audioSource.isPlaying) return;

        AudioClip soundToPlay = null;
        if (isAttacking && attackSound != null)
            soundToPlay = attackSound;
        else if (isChasing && chaseSound != null)
            soundToPlay = chaseSound;
        else if (idleSound != null)
            soundToPlay = idleSound;

        if (soundToPlay != null)
            audioSource.PlayOneShot(soundToPlay);
    }

    void ResetSoundTimer()
    {
        soundTimer = 0f;
        nextSoundTime = Random.Range(minSoundDelay, maxSoundDelay);
    }

    // -------------------------
    //      ESTADOS
    // -------------------------

    void Patrol()
    {
        isChasing = false;
        isAttacking = false;

        // animación caminar
        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);

        // cambiar dirección cada cierto tiempo
        directionTimer -= Time.deltaTime;
        if (directionTimer <= 0)
            ChooseNewPatrolDirection();

        rb.linearVelocity = patrolDirection * speed;
    }

    void StartChasing()
    {
        isChasing = true;
        isAttacking = false;

        animator.SetBool("isWalking", true);
        animator.SetBool("isAttacking", false);

        Vector2 dir = (player.position - transform.position).normalized;
        rb.linearVelocity = dir * speed;
    }

    void StartAttack()
    {
        isChasing = false;
        isAttacking = true;

        // animación atacar
        animator.SetBool("isWalking", false);
        animator.SetBool("isAttacking", true);

        rb.linearVelocity = Vector2.zero;

        // Ataque automático cuando está en rango
        if (canDamage)
        {
            DealDamage();
        }
    }

    // -------------------------
    //  DIRECCIÓN ALEATORIA
    // -------------------------

    void ChooseNewPatrolDirection()
    {
        patrolDirection = Random.insideUnitCircle.normalized;
        directionTimer = directionChangeInterval;
    }

    // -------------------------
    //  COLISIÓN CON PAREDES
    // -------------------------

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isChasing && !isAttacking)
        {
            // Cambia a una nueva dirección aleatoria al chocar
            ChooseNewPatrolDirection();
        }
    }

    // -------------------------
    //      FLIP SPRITE
    // -------------------------

    void HandleFlip()
    {
        if (rb.linearVelocity.x > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (rb.linearVelocity.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }

    // -------------------------
    //      Sistema de daño
    // -------------------------

    public void DealDamage()
    {
        if (!canDamage || player == null) return;

        float distance = Vector2.Distance(transform.position, player.position);

        // Verificar si el jugador está en rango de ataque
        if (distance <= attackRange)
        {
            if (player.TryGetComponent<PlayerHealth>(out var health))
            {
                health.TakeDamage(damage);
                Debug.Log($"Zombie atacó al jugador: {damage} de daño");
            }

            StartCoroutine(DamageCooldown());
        }

        if (audioSource != null && attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    IEnumerator DamageCooldown()
    {
        canDamage = false;
        yield return new WaitForSeconds(attackCooldown);
        canDamage = true;
    }
}