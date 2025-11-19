# Unity 6 + ChatGPT Integration Guide

This guide helps you wire the ChatGPT API into a Unity 6 project so characters can discuss school-related issues inside your application.

## Prerequisites
- Unity 6 (or newer) installed with .NET 8-compatible scripting backend.
- An OpenAI API key with access to GPT-4.1 or GPT-4o class models.
- Git installed for version control.
- Optional: Node 18+ if you want to run local mock endpoints for tests.

## Repository structure
The `unity/` directory contains example scripts and configuration helpers that you can copy into your Unity project. They are intentionally lightweight and do not require external packages.

```
unity/
  Assets/
    Scripts/
      ChatGPTClient.cs          # Handles HTTPS calls to the Chat Completions API.
      CharacterChatController.cs# Example conversation orchestrator for NPCs.
```

## Setting API credentials
Unity builds should never hardcode your API key in source control. The sample scripts load the key from:

1. **Environment variable** `OPENAI_API_KEY` (preferred for development builds).
2. **PlayerPrefs** key `OpenAI_API_Key` if set at runtime via the `ConfigureKey` method.

For editor testing, you can set the variable inside Unity:

```
Edit > Project Settings > Player > Scripting Define Symbols
# Add OPENAI_API_KEY=your_key_here (or set it in your OS shell before launching Unity)
```

## Adding the scripts to your Unity project
1. Copy the files from `unity/Assets/Scripts/` into your Unity project’s `Assets/Scripts/` folder.
2. In your scene, create an empty GameObject (e.g., `ChatServices`) and attach **ChatGPTClient** to it.
3. Attach **CharacterChatController** to each NPC GameObject that should speak with the player.
4. Wire your UI (input field, send button, and output text) to the serialized fields on `CharacterChatController` in the Inspector.

## How the sample works
- `ChatGPTClient` exposes a coroutine `SendChat` that accepts a message history and returns the assistant reply.
- `CharacterChatController` keeps a running conversation, seeds it with a persona prompt, and updates the UI when replies arrive.
- Requests target the Chat Completions API (`/v1/chat/completions`) with the `gpt-4.1-mini` model by default. You can swap it for any model you are provisioned for.

## Editor play-mode flow
1. Enter play mode.
2. Type a student concern into the input box and click your bound Send button.
3. The controller pushes the user message, calls the ChatGPT API via the client, and appends the assistant’s reply to the output text.

## Troubleshooting
- **401 Unauthorized:** Confirm the API key is set in your OS environment or via `ConfigureKey()` before sending a request.
- **429 Rate limit:** Reduce request frequency or add retry/backoff before resubmitting.
- **Empty replies:** Check Unity Console logs for deserialization errors; ensure the response JSON matches the sample response type in `ChatGPTClient.cs`.
- **WebGL builds:** UnityWebRequest works, but confirm CORS is permitted for your deployment host; consider a small relay service if needed.

## Next steps
- Persist chat history across sessions by writing the `messages` list to disk before scene unload.
- Add safety filters by pre-validating user input or injecting a system prompt that enforces school policy.
- Cache persona prompts and model choices in ScriptableObjects so designers can tweak behavior per character without code changes.
