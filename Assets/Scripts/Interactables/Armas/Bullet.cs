using UnityEngine;

public class Bullet : MonoBehaviour
{
    [HideInInspector] public int damage = 10;
    [HideInInspector] public float speed = 15f;
    public GameObject impactEffect;

    void Update()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Bullet")) return;

        if (other.CompareTag("Zombie"))
        {
            ZombieHealth zh = other.GetComponent<ZombieHealth>();
            if (zh != null)
                zh.TakeDamage(damage);
        }

        if (impactEffect != null)
            Instantiate(impactEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}

