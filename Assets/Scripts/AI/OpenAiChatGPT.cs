using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace AI
{
    public class OpenAIChatGPT : MonoBehaviour
    {
        [SerializeField]
        private TextAsset apiKeyFile;
        private string apiKey;
        private string apiURL = "https://api.openai.com/v1/chat/completions";

        public string riddle = "";
        // Update this to pass dynamically
        public string landMarks;
        public string chestLocations;
        public string playerPosition;

        [Serializable]
        public class RootObject
        {
            public string id;
            public string obj;
            public long created;
            public string model;
            public Choice[] choices;
            public Usage usage;
            public string system_fingerprint;
        }
        [Serializable]
        public class Usage
        {
            public int prompt_tokens;
            public int completion_tokens;
            public int total_tokens;
        }

        [Serializable]
        public class Choice
        {
            public int index;
            public Message message;
            public object logprobs; 
            public string finish_reason;
        }
         void Start()
        {
            LoadApiKey();
        }

        private void LoadApiKey()
        {
            if (apiKeyFile != null)
            {
                apiKey = apiKeyFile.text.Trim();
            }
            else
            {
                Debug.LogError("API key file not assigned!");
            }
        }

        IEnumerator CreateRiddle(string prompt, GameManager mgmt)
        {
            
            ChatRequest requestBody = new ChatRequest
            {
                model = "gpt-3.5-turbo-0125",
                messages = new Message[]
                {
                    new Message { role = "user", content = prompt }
                }
            };

            string json = JsonUtility.ToJson(requestBody);

            // Setup the UnityWebRequest
            var request = new UnityWebRequest(apiURL, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + apiKey);

            // Send the request
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                
                Debug.Log(request.downloadHandler.text);
                riddle = ExtractContent(request.downloadHandler.text);
                mgmt.Riddle = riddle;
                Debug.Log(riddle);
               
            }
            else
            {
                Debug.LogError("Error: " + request.error);
            }
        }

        public IEnumerator RequestRiddle(string landmarks, string chestLocation, string playerPosition, string difficulty, GameManager mgmt)
        {
            this.landMarks = landmarks;
            this.chestLocations = chestLocation;
            this.playerPosition = playerPosition;
            string prompt = $"Give me a riddle for the treasure chest. I have given you the landmark locations {landMarks} and chest location {chestLocations} and player position {playerPosition}, where -x is west, x is east, -z is south and z is north, filter through the landmark locations and find relevant locations and give me the 4 line riddle, Difficulty: {difficulty}, never give player the exact coordinates or coordinates of any sort, and make the riddle relative, dont include objects that are far in the riddle even if that may make the riddle shorter only include relatively close landmarks, also note that if the chest is below y -1 it is on sand, if it is above -1 and below +0.5 its on the grassy plains use this part in the riddle if possible, use that land mark the relatively close land marks will be with 2.5 units of the chest, mid distance will be 10 units and far will be more than that, make sure you include the landmarks based on this relative proximity, never mention the players location, that is for your reference to make the riddle, you can give them the direction based on their location, but dont straight up say the players location IMPORTANT: the land mark names are the keywords, dont include them if they are not relatively close as it can confuse the player, More important: only include landmarks in the riddle if they are within 2.5 unit distance of the treasure chest, if not possible you may extend the search till 5 unit, but then point the direction from the landmark and use keywords such as a bit off form the landmark in ... direction";
            yield return StartCoroutine(CreateRiddle(prompt, mgmt));
        
        }
        string ExtractContent(string json)
        {
            RootObject root = JsonUtility.FromJson<RootObject>(json);
            string content = root.choices[0].message.content;
            content = content.Replace("\\n", "");
            return content;
        }
    }
}
