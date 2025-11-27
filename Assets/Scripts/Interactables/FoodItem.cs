using UnityEngine;

public class FoodItem : MonoBehaviour
{
    [Header("Configuración de Comida")]
    public float energyRestore = 25f;

    [Header("Efectos")]
    public GameObject collectionEffect;
    public AudioClip collectionSound;

    private bool isCollected = false;
    private SpriteRenderer spriteRenderer;
    private Collider2D foodCollider;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        foodCollider = GetComponent<Collider2D>();

        // Asegurar que tenga tag
        gameObject.tag = "Food";

        // Configurar collider como trigger si no lo está
        if (foodCollider != null)
        {
            foodCollider.isTrigger = true;
        }
        else
        {
            Debug.LogError("No hay Collider2D en la comida: " + gameObject.name);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;

        Debug.Log("Trigger entered with: " + other.gameObject.name);

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detectado, intentando recoger comida...");
            CollectFood(other.gameObject);
        }
    }

    void CollectFood(GameObject player)
    {
        if (isCollected) return;

        isCollected = true;

        PlayerEnergy playerEnergy = player.GetComponent<PlayerEnergy>();
        if (playerEnergy != null)
        {
            Debug.Log("PlayerEnergy encontrado, agregando energía: " + energyRestore);
            playerEnergy.AddEnergy(energyRestore);

            // Efectos visuales y de sonido
            PlayCollectionEffects();

            // Ocultar comida inmediatamente
            if (spriteRenderer != null)
                spriteRenderer.enabled = false;
            if (foodCollider != null)
                foodCollider.enabled = false;

            // Destruir después de un tiempo
            Destroy(gameObject, 2f);
        }
        else
        {
            Debug.LogError("No se encontró PlayerEnergy en el player!");
        }
    }

    void PlayCollectionEffects()
    {
        // Efecto de partículas
        if (collectionEffect != null)
        {
            Instantiate(collectionEffect, transform.position, Quaternion.identity);
        }

        // Sonido
        if (collectionSound != null)
        {
            AudioSource.PlayClipAtPoint(collectionSound, transform.position);
        }
    }
}