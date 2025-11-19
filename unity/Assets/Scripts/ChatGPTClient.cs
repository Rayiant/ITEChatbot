using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace ITEChatbot
{
    [Serializable]
    public class ChatMessage
    {
        public string role;
        public string content;

        public ChatMessage(string role, string content)
        {
            this.role = role;
            this.content = content;
        }
    }

    [Serializable]
    public class ChatRequest
    {
        public string model = "gpt-4.1-mini";
        public List<ChatMessage> messages;
        public float temperature = 0.7f;

        public ChatRequest(List<ChatMessage> messages, string model = "gpt-4.1-mini", float temperature = 0.7f)
        {
            this.messages = messages;
            this.model = model;
            this.temperature = temperature;
        }
    }

    [Serializable]
    public class ChatChoice
    {
        public int index;
        public ChatMessage message;
    }

    [Serializable]
    public class ChatResponse
    {
        public List<ChatChoice> choices;
    }

    public class ChatGPTClient : MonoBehaviour
    {
        [SerializeField]
        private string apiKey;

        [SerializeField]
        private string model = "gpt-4.1-mini";

        [SerializeField]
        private float temperature = 0.7f;

        private const string ApiUrl = "https://api.openai.com/v1/chat/completions";
        private const string PlayerPrefsKey = "OpenAI_API_Key";

        private void Awake()
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
            }

            if (string.IsNullOrWhiteSpace(apiKey) && PlayerPrefs.HasKey(PlayerPrefsKey))
            {
                apiKey = PlayerPrefs.GetString(PlayerPrefsKey);
            }
        }

        public void ConfigureKey(string key)
        {
            apiKey = key;
            PlayerPrefs.SetString(PlayerPrefsKey, key);
        }

        public IEnumerator SendChat(List<ChatMessage> messages, Action<string> onComplete, Action<string> onError = null)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                onError?.Invoke("OpenAI API key is missing. Set OPENAI_API_KEY or call ConfigureKey().");
                yield break;
            }

            var requestBody = new ChatRequest(messages, model, temperature);
            var json = JsonUtility.ToJson(requestBody);
            var requestData = Encoding.UTF8.GetBytes(json);

            using (var request = new UnityWebRequest(ApiUrl, UnityWebRequest.kHttpVerbPOST))
            {
                request.uploadHandler = new UploadHandlerRaw(requestData);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", $"Bearer {apiKey}");

                yield return request.SendWebRequest();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    onError?.Invoke($"Request failed: {request.responseCode} {request.error}\n{request.downloadHandler.text}");
                    yield break;
                }

                var responseJson = request.downloadHandler.text;
                var response = JsonUtility.FromJson<ChatResponse>(responseJson);

                if (response?.choices != null && response.choices.Count > 0 && response.choices[0].message != null)
                {
                    onComplete?.Invoke(response.choices[0].message.content);
                }
                else
                {
                    onError?.Invoke("OpenAI response did not contain a message.");
                }
            }
        }
    }
}
