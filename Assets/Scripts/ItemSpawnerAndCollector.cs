using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnerAndCollector : MonoBehaviour
{
    [Header(" Prefabs de objetos a generar")]
    public GameObject[] itemPrefabs; // los objetos que puede generar

    [Header("Configuración de generación")]
    public int itemCount = 5; // cuántos objetos generar
    public Vector2 spawnAreaMin = new Vector2(-5, -3);
    public Vector2 spawnAreaMax = new Vector2(5, 3);

    [Header("Configuración de interacción")]
    public string itemTag = "Item"; // Tag de los objetos recolectables
    public float pickupRange = 1f; // distancia máxima para recoger
    public KeyCode pickupKey = KeyCode.X;

    [Header("Inventario (temporal en memoria)")]
    public List<GameObject> inventory = new List<GameObject>();

    private void Start()
    {
        SpawnRandomItems();
    }

    void SpawnRandomItems()
    {
        if (itemPrefabs.Length == 0)
        {
            Debug.LogWarning("No hay prefabs asignados para generar.");
            return;
        }

        for (int i = 0; i < itemCount; i++)
        {
            Vector2 randomPos = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );

            GameObject prefab = itemPrefabs[Random.Range(0, itemPrefabs.Length)];
            Instantiate(prefab, randomPos, Quaternion.identity);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            TryPickupItem();
        }
    }

    void TryPickupItem()
    {
        // Busca los objetos cercanos con el tag especificado
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pickupRange);

        foreach (var hit in hits)
        {
            if (hit.CompareTag(itemTag))
            {
                // Lo guarda en el inventario
                inventory.Add(hit.gameObject);
                hit.gameObject.SetActive(false); // lo "guarda", sin destruirlo
                Debug.Log("Objeto guardado en inventario: " + hit.name);
                return; // solo recoge uno a la vez
            }
        }

        Debug.Log("No hay objetos cercanos para recoger.");
    }

    // Solo para visualizar el rango de recogida en la vista de escena
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
