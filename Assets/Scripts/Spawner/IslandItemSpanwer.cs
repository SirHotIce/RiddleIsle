using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Spawner
{
    public class IslandItemSpawner : MonoBehaviour
    {
        [System.Serializable]
        public class Spawnable
        {
            public GameObject prefab;
            public float minSize = 1.0f;
            public float maxSize = 1.0f;
            public bool canSpawnOnSand;
            public bool canSpawnOnRock;
            public bool canSpawnOnGrass;
        }

        public List<Spawnable> spawnables = new List<Spawnable>();
        public int minCount = 1;
        public int maxCount = 5;
        public Transform islandTransform;
        public LayerMask terrainMask;  // Layer mask for terrain

        
        public string spawnedItemsInfo = "";

        public IEnumerator SpawnItems()
        {
            Debug.Log($"Starting to spawn items, spawnables count: {spawnables.Count}");
            foreach (var item in spawnables)
            {
                Debug.Log($"Processing {item.prefab.name}");
                int count = Random.Range(minCount, maxCount + 1);
                for (int i = 0; i < count; i++)
                {
                    Vector3 position = GenerateRandomPosition(item);
                    if (position == Vector3.zero)
                    {
                        Debug.Log("Invalid position, continuing to next");
                        continue;
                    }
                    GameObject obj = Instantiate(item.prefab, position, Quaternion.identity, islandTransform);
                    float scale = Random.Range(item.minSize, item.maxSize);
                    obj.transform.localScale = new Vector3(scale, scale, scale);
                    obj.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
                    AlignWithTerrain(obj);
                    print($"Trying to spawn {item.prefab.name} at {position}");

                    // Store item type and position in the data string
                    spawnedItemsInfo += $"{item.prefab.name}: {position.x}, {position.y}, {position.z}; ";

                    yield return null;
                }
            }
            Debug.Log("Spawning Complete");

            
            FindObjectOfType<GameManager>().LandMark = $"LandMark Locations: {spawnedItemsInfo}";
            Debug.Log($"Spawned Items: {spawnedItemsInfo}");
        }

        private Vector3 GenerateRandomPosition(Spawnable spawnable)
        {
            int attempts = 10;
            float maxTerrainHeight = 200;  
            while (attempts-- > 0)
            {
                Vector3 position = new Vector3(Random.Range(-100f, 100f), maxTerrainHeight, Random.Range(-100f, 100f));
                RaycastHit hit;

                // Draw ray for debugging
                Debug.DrawRay(position, Vector3.down * (maxTerrainHeight + 100), Color.red, 10f);

                if (Physics.Raycast(position, Vector3.down, out hit, maxTerrainHeight + 100, terrainMask))
                {
                    Debug.Log($"Raycast hit {hit.collider.gameObject.name} at {hit.point}, terrain mask {terrainMask.value}");
                    float y = hit.point.y;
                    if ((y > -1 && spawnable.canSpawnOnSand) ||
                        (y <= -1 && y > 0.5 && spawnable.canSpawnOnGrass) ||
                        (y > 0.5 && spawnable.canSpawnOnRock))
                    {
                        return hit.point;
                    }
                }
                else
                {
                    Debug.Log("Raycast did not hit any valid terrain on this attempt");
                }
            }
            Debug.Log("No valid positions found after 10 attempts");
            return Vector3.zero;
        }

        private void AlignWithTerrain(GameObject obj)
        {
            if (Physics.Raycast(obj.transform.position + Vector3.up * 10, Vector3.down, out var hit))
            {
                obj.transform.position = hit.point;
                obj.transform.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * obj.transform.rotation;
            }
        }
    }
}
