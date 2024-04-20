using UnityEngine;

namespace Manager
{
    public class TreasureFinder : MonoBehaviour
    {
        private Vector3 chestLocation;
        public GameObject successEffect;
        public GameObject invalidEffect;
        public GameObject failEffect;
        public float tolerance = 10f;
        private Camera mainCamera;
        public LayerMask islandLayer;
        public LayerMask oceanLayer;
        private bool initiated;

        public AudioClip successSound; 
        public AudioClip failSound; 
        private AudioSource audioSource; 

        public void Initiate(Vector3 chestAt)
        {
            initiated = true;
            chestLocation = chestAt;
            mainCamera = Camera.main;
            audioSource = GetComponent<AudioSource>(); 
            if (audioSource == null) { 
                audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        private void Update()
        {
            if (initiated)
            {
                if (Input.GetMouseButtonDown(0)) 
                {
                    GameManager gameManager = FindObjectOfType<GameManager>();
                    if (!gameManager.Paused)
                    {
                        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit))
                        {
                            ProcessClick(hit);
                        }
                    }
                }
            }
        }

        private void ProcessClick(RaycastHit hit)
        {
            if ((islandLayer.value & (1 << hit.collider.gameObject.layer)) != 0) // Check if the hit is on the island layer
            {
                float distance = Vector3.Distance(hit.point, chestLocation);
                if (distance <= tolerance)
                {
                    HandleSuccessfulFind(hit.point);
                }
                else
                {
                    GameManager gameManager = FindObjectOfType<GameManager>();
                    gameManager.DecreaseLives();
                    Instantiate(invalidEffect, hit.point, Quaternion.identity);
                    PlaySound(failSound);
                }
            }
            else if ((oceanLayer.value & (1 << hit.collider.gameObject.layer)) != 0) // Check if the hit is on the ocean layer
            {
                Instantiate(failEffect, hit.point, Quaternion.identity);
                PlaySound(failSound);
            }
        }

        private void HandleSuccessfulFind(Vector3 position)
        {
            GameManager gameManager = FindObjectOfType<GameManager>();
            int score = Mathf.RoundToInt((1 - (position - chestLocation).magnitude / tolerance) * 1000);
            score *= gameManager.LivesLeft;
            gameManager.IncreaseScore(score);
            Instantiate(successEffect, position, Quaternion.identity);
            PlaySound(successSound);
            Debug.Log($"Treasure found! Score awarded: {score}");
        }

        private void PlaySound(AudioClip clip)
        {
            if (audioSource && clip)
            {
                audioSource.PlayOneShot(clip);
            }
        }
    }
}
