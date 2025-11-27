using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 10;
    public float speed = 15f;
    public GameObject impactEffect;

    private Vector2 startPosition;
    private bool hasHit = false;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (!hasHit)
        {
            // Movimiento continuo
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit) return;

        // Ignorar colisiones con el player y otras balas
        if (other.CompareTag("Player") || other.CompareTag("Bullet")) return;

        hasHit = true;

        // Aplicar daño a zombies
        if (other.CompareTag("Zombie"))
        {
            ZombieHealth zombieHealth = other.GetComponent<ZombieHealth>();
            if (zombieHealth != null)
            {
                zombieHealth.TakeDamage(damage);
                Debug.Log($"Bala impactó zombie: {damage} de daño");
            }
        }

        // Efecto de impacto (opcional)
        if (impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        // Hacer invisible inmediatamente y destruir después
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(gameObject, 0.1f);
    }
}