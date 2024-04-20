using System.Collections;
using UnityEngine;

namespace Spawner
{
    public class ChestSpawner : MonoBehaviour
    {
        public GameObject chestPrefab;
        public Transform islandTransform;
        public LayerMask terrainMask;
        public Vector3 chestPosition;

        public IEnumerator SpawnChest()
        {
            chestPosition = GenerateRandomPositionForChest();
            if (chestPosition != Vector3.zero)
            {
                GameObject chest = Instantiate(chestPrefab, chestPosition, Quaternion.identity, islandTransform);
                AlignWithTerrain(chest);
                GameManager manager = FindObjectOfType<GameManager>();
                manager.Chest = $"Spawned treasure chest at: {chestPosition}";
                manager.ChestLocation = chestPosition;
                Debug.Log($"Spawned treasure chest at: {chestPosition}");
                chest.SetActive(false);
            }
            else
            {
                Debug.Log("Failed to find a valid position for the treasure chest.");
                
            }
            yield return null;
        }

        private Vector3 GenerateRandomPositionForChest()
        {
            int maxAttempts = 1000; 
            float maxTerrainHeight = 200;
            Vector3 position = Vector3.zero;
            for (int i = 0; i < maxAttempts; i++)
            {
                position = new Vector3(Random.Range(-100f, 100f), maxTerrainHeight, Random.Range(-100f, 100f));
                if (TryGetPosition(position, out Vector3 foundPosition))
                {
                    return foundPosition;
                }
            }
            return Vector3.zero; // Return zero if no valid position found
        }

        private bool TryGetPosition(Vector3 position, out Vector3 foundPosition)
        {
            RaycastHit hit;
            if (Physics.Raycast(position, Vector3.down, out hit, 300, terrainMask))
            {
                if (hit.point.y > -2)
                {
                    foundPosition = hit.point;
                    return true;
                }
            }
            foundPosition = Vector3.zero;
            return false;
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
