using UnityEngine;
using System.Collections.Generic;

public class ZombieSpawner : MonoBehaviour
{
    [Header("Configuración de Spawn")]
    public GameObject zombiePrefab;
    public int maxZombies = 10;
    public float spawnRadius = 15f;
    public float minDistanceFromPlayer = 8f;
    public float spawnInterval = 3f;

    [Header("Zona de Spawn")]
    public bool useSpawnZone = true;
    public Collider2D spawnZoneCollider; 

    private List<GameObject> activeZombies = new List<GameObject>();
    private Transform player;
    private float spawnTimer;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;

        
        for (int i = 0; i < maxZombies / 2; i++)
        {
            SpawnZombie();
        }
    }

    void Update()
    {
        
        activeZombies.RemoveAll(zombie => zombie == null);

        
        if (activeZombies.Count < maxZombies)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnInterval)
            {
                SpawnZombie();
                spawnTimer = 0f;
            }
        }
    }

    void SpawnZombie()
    {
        Vector2 spawnPosition = GetValidSpawnPosition();

        if (spawnPosition != Vector2.zero)
        {
            GameObject zombie = Instantiate(zombiePrefab, spawnPosition, Quaternion.identity);
            activeZombies.Add(zombie);

            
            zombie.transform.SetParent(transform);
        }
    }

    Vector2 GetValidSpawnPosition()
    {
        int attempts = 0;
        int maxAttempts = 30;

        while (attempts < maxAttempts)
        {
            Vector2 spawnPos;

            if (useSpawnZone && spawnZoneCollider != null)
            {
                
                spawnPos = GetRandomPointInCollider(spawnZoneCollider);
            }
            else
            {
               
                spawnPos = (Vector2)player.position + Random.insideUnitCircle.normalized * spawnRadius;
            }

            
            if (Vector2.Distance(spawnPos, player.position) >= minDistanceFromPlayer)
            {
                
                Collider2D[] colliders = Physics2D.OverlapCircleAll(spawnPos, 1f);
                bool validPosition = true;

                foreach (Collider2D collider in colliders)
                {
                    if (collider.CompareTag("Zombie"))
                    {
                        validPosition = false;
                        break;
                    }
                }

                if (validPosition)
                {
                    return spawnPos;
                }
            }

            attempts++;
        }

        Debug.LogWarning("No se pudo encontrar posición válida para spawnear zombie");
        return Vector2.zero;
    }

    Vector2 GetRandomPointInCollider(Collider2D collider)
    {
        if (collider is BoxCollider2D boxCollider)
        {
            Vector2 center = boxCollider.bounds.center;
            Vector2 size = boxCollider.bounds.size;

            float x = Random.Range(center.x - size.x / 2, center.x + size.x / 2);
            float y = Random.Range(center.y - size.y / 2, center.y + size.y / 2);

            return new Vector2(x, y);
        }
        else if (collider is PolygonCollider2D polyCollider)
        {
           
            Bounds bounds = polyCollider.bounds;
            int attempts = 0;

            while (attempts < 50)
            {
                Vector2 point = new Vector2(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y)
                );

                if (polyCollider.OverlapPoint(point))
                    return point;

                attempts++;
            }
        }

        return (Vector2)transform.position + Random.insideUnitCircle * 5f;
    }

    
    public void ForceSpawn(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (activeZombies.Count < maxZombies)
                SpawnZombie();
        }
    }

    
    public void ClearAllZombies()
    {
        foreach (GameObject zombie in activeZombies)
        {
            if (zombie != null)
                Destroy(zombie);
        }
        activeZombies.Clear();
    }

    
    private void OnDrawGizmosSelected()
    {
        
        Gizmos.color = Color.red;

        if (useSpawnZone && spawnZoneCollider != null)
        {
            if (spawnZoneCollider is BoxCollider2D box)
            {
                Gizmos.DrawWireCube(box.bounds.center, box.bounds.size);
            }
            else if (spawnZoneCollider is PolygonCollider2D poly)
            {
                Gizmos.DrawWireCube(poly.bounds.center, poly.bounds.size);
            }
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position, spawnRadius);
        }

        
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(player.position, minDistanceFromPlayer);
        }
    }

    public void StartSpawning()
    {
        enabled = true;
        // Reiniciar timer de spawn si es necesario
        spawnTimer = 0f;
    }

    public void StopSpawning()
    {
        enabled = false;
    }
}