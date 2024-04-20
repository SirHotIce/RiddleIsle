using System.Collections;
using System.Collections.Generic;
using AI;
using Manager;
using Noise_Stuff;
using Spawner;
using UnityEngine;

namespace ProcManagement
{
    public class ProcManager : MonoBehaviour
    {
        [SerializeField] private GameObject landMass;
        [SerializeField] private LoadingBarManagement loadingSystem;
        
        void Start()
        {
            loadingSystem = FindObjectOfType<LoadingBarManagement>();
            StartCoroutine(Processes());
        }

        IEnumerator Processes()
        {
            var gameManager = FindObjectOfType<GameManager>();
            loadingSystem.Value = 0.25f;
            gameManager.DiffScaler();
            print("Started");
            yield return StartCoroutine(landMass.GetComponent<ProcMeshGenerator>().Gen(gameManager.DiffScale));
            print("Island Generated");
            loadingSystem.Value = 0.50f;

            yield return StartCoroutine(landMass.GetComponent<IslandItemSpawner>().SpawnItems());
            print("All Items in Place");
            loadingSystem.Value = 0.65f;

            yield return StartCoroutine(landMass.GetComponent<ChestSpawner>().SpawnChest());
            print("Chest Spawned");
            loadingSystem.Value = 0.75f;

            
            if (gameManager.UseGpt)
            {
                if (Application.internetReachability != NetworkReachability.NotReachable)
                {
                    print("Connected to network");
                    yield return StartCoroutine(FindObjectOfType<OpenAIChatGPT>()
                        .RequestRiddle(gameManager.landMarks, gameManager.chest, $"Player is at 0,0,0",
                            gameManager.Diff.ToString(), gameManager));
                }
                else
                {
                    print("Network not reachable");
                }
            }
            else
            {
                print("Gpt is Disabled");
                gameManager.Riddle = "GPT is Disabled";
            }


            gameManager.PublishRiddle();
            print("All Processes Completed");
            loadingSystem.Value = 0.90f;
            

            Invoke(nameof(DisableLoader),0.5f);
            loadingSystem.Value = 1f;
            


        }

        private void DisableLoader()
        {
            Destroy(loadingSystem.gameObject);
            //loadingSystem.gameObject.SetActive(false);
        }
        
    }
}
