using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ITEChatbot
{
    public class CharacterChatController : MonoBehaviour
    {
        [TextArea]
        [SerializeField]
        private string characterPersona = "You are a supportive school counselor helping students navigate challenges.";

        [SerializeField]
        private ChatGPTClient chatClient;

        [SerializeField]
        private InputField inputField;

        [SerializeField]
        private Text transcriptText;

        private readonly List<ChatMessage> messages = new List<ChatMessage>();
        private bool isRequestInFlight;

        private void Start()
        {
            messages.Clear();
            messages.Add(new ChatMessage("system", characterPersona));
        }

        public void OnSendClicked()
        {
            if (isRequestInFlight || inputField == null || chatClient == null)
            {
                return;
            }

            var userMessage = inputField.text.Trim();
            if (string.IsNullOrEmpty(userMessage))
            {
                return;
            }

            AppendToTranscript($"You: {userMessage}\n");
            messages.Add(new ChatMessage("user", userMessage));
            inputField.text = string.Empty;

            StartCoroutine(SendAndRenderResponse());
        }

        private IEnumerator SendAndRenderResponse()
        {
            isRequestInFlight = true;

            yield return chatClient.SendChat(
                messages,
                onComplete: reply =>
                {
                    messages.Add(new ChatMessage("assistant", reply));
                    AppendToTranscript($"Counselor: {reply}\n\n");
                    isRequestInFlight = false;
                },
                onError: error =>
                {
                    AppendToTranscript($"[Error] {error}\n");
                    isRequestInFlight = false;
                });
        }

        private void AppendToTranscript(string text)
        {
            if (transcriptText != null)
            {
                transcriptText.text += text;
            }
            else
            {
                Debug.Log(text);
            }
        }
    }
}
