using UnityEngine;

public class FoodSpawner : MonoBehaviour
{
    [Header("Prefab de comida")]
    public GameObject foodPrefab;

    [Header("Área de spawn")]
    public Vector2 areaSize = new Vector2(10f, 10f); 
    public Vector2 areaOffset = Vector2.zero;        

    [Header("Configuración de spawn")]
    public float spawnInterval = 5f; 
    public int maxFood = 10;         

    private int currentFoodCount = 0;

    void Start()
    {
        // Inicia el spawn automático
        InvokeRepeating(nameof(SpawnFood), 1f, spawnInterval);
    }

    void SpawnFood()
    {
        if (foodPrefab == null) return;
        if (currentFoodCount >= maxFood) return;

        // Posición aleatoria dentro del área
        Vector3 spawnPos = transform.position + new Vector3(
            Random.Range(-areaSize.x / 2f, areaSize.x / 2f) + areaOffset.x,
            Random.Range(-areaSize.y / 2f, areaSize.y / 2f) + areaOffset.y,
            0f
        );

        GameObject food = Instantiate(foodPrefab, spawnPos, Quaternion.identity);
        currentFoodCount++;
    }
}

