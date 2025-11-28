using UnityEngine;

public class FoodItem : MonoBehaviour
{
    [Header("Configuración de Comida")]
    public float energyRestore = 25f;
    [Range(0f, 1f)] public float poisonChance = 0.2f; // probabilidad de estar envenenada

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

        gameObject.tag = "Food";

        if (foodCollider != null)
            foodCollider.isTrigger = true;
        else
            Debug.LogError("No hay Collider2D en la comida: " + gameObject.name);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isCollected) return;

        if (other.CompareTag("Player"))
        {
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
            bool isPoisoned = Random.value < poisonChance;

            if (isPoisoned)
            {
                playerEnergy.currentEnergy -= energyRestore; // le resta energía
                playerEnergy.currentEnergy = Mathf.Clamp(playerEnergy.currentEnergy, 0, playerEnergy.maxEnergy);
                Debug.Log($"¡Comida envenenada! Pierdes {energyRestore} de energía.");
            }
            else
            {
                playerEnergy.AddEnergy(energyRestore); // suma energía normalmente
                Debug.Log($"Comida buena! Recuperas {energyRestore} de energía.");
            }

            PlayCollectionEffects();

            if (spriteRenderer != null) spriteRenderer.enabled = false;
            if (foodCollider != null) foodCollider.enabled = false;

            Destroy(gameObject, 2f);
        }
        else
        {
            Debug.LogError("No se encontró PlayerEnergy en el player!");
        }
    }

    void PlayCollectionEffects()
    {
        if (collectionEffect != null)
            Instantiate(collectionEffect, transform.position, Quaternion.identity);

        if (collectionSound != null)
            AudioSource.PlayClipAtPoint(collectionSound, transform.position);
    }
}
