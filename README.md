# ITEChatbot

A chatbot for ITE students to complain or raise concerns.

## Getting started with Unity 6 + ChatGPT
This repository now includes sample Unity 6 scripts and setup instructions to connect NPC dialogue to the OpenAI ChatGPT API.

- Read the full integration guide: [`docs/UnityIntegration.md`](docs/UnityIntegration.md)
- Copy the scripts in `unity/Assets/Scripts/` into your Unity project.
- Set your `OPENAI_API_KEY` environment variable before entering Play mode, or call `ConfigureKey()` at runtime.

## Repository layout
- `docs/UnityIntegration.md` – step-by-step instructions for wiring the ChatGPT API into Unity 6.
- `unity/Assets/Scripts/` – drop-in C# scripts (`ChatGPTClient.cs` and `CharacterChatController.cs`) to send chat completions and display responses.
