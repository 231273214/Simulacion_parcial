using UnityEngine;

public class NPCFlee : MonoBehaviour
{
    [Header("Stats")]
    public float moveSpeed = 2f;           // velocidad al huir
    public float visionRange = 3f;         // distancia a la que detecta zombies

    private Rigidbody2D rb;
    private Transform threat;               // el zombie más cercano
    private Vector2 fleeDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Buscar zombies en rango
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, visionRange);
        threat = null;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Zombie")) // asegurarte de que tus zombies tengan tag "Zombie"
            {
                threat = hit.transform;
                break; // solo detecta el primero
            }
        }

        if (threat != null)
        {
            // Huir del zombie
            fleeDirection = (transform.position - threat.position).normalized;
            rb.velocity = fleeDirection * moveSpeed;
        }
        else
        {
            // Sin amenaza, quedarse quieto
            rb.velocity = Vector2.zero;
        }
    }

    // Opcional: dibujar el rango de visión en el editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, visionRange);
    }
}

