# Azure OpenAI — Hands-On Learning Project (.NET 10 / C#)

A modular .NET 10 solution that demonstrates end-to-end integration with **Azure OpenAI** across chat, function calling, embeddings, image generation, and audio transcription. Built while completing an Azure OpenAI Udemy course to practice each capability with clean, testable, production-style code.

> **Tech stack:** .NET 10 · C# · `Azure.AI.OpenAI` SDK · NUnit · Visual Studio 2026

---

## Solution Structure

| Project | Description |
|--------|-------------|
| `Azure.OpenAI` | Core class library wrapping the Azure OpenAI SDK, one service per capability. |
| `Azure.OpenAI.Test` | NUnit test project exercising every service, including integration tests. |

Each feature follows the same pattern: an interface (`I...Service`) + implementation, constructor injection of the `AzureOpenAIClient`, and defensive input validation.

---

## Capabilities Implemented

### 💬 Chat Completions — `ChatService`
- Standard prompt/response chat completions.
- **Streaming** responses for real-time token output.
- Configurable model parameters (Temperature, MaxOutputTokenCount, TopP, Frequency/Presence penalties).

### 🛠️ Function / Tool Calling — `FunctionCallingService`
- Full multi-turn tool-calling loop: detects model tool requests, invokes the tool, feeds results back, and iterates until a final answer.
- Safety cap on tool iterations to prevent infinite loops.
- Reusable `ChatToolCatalog` for defining tools in tests.

### 🔎 Embeddings & Semantic Similarity — `EmbeddingService` + `VectorMath`
- Generates text embeddings via Azure OpenAI.
- Hand-implemented **cosine similarity** for ranking/comparing text by meaning.

### 🖼️ Image Generation — `ImageGenerationService`
- Generates images from text prompts and saves the returned bytes to disk.

### 🎙️ Audio Transcription — `AudioTranscriptionService`
- Speech-to-text transcription of audio files (Whisper).

---

## Configuration

Configuration is layered via `Microsoft.Extensions.Configuration`:

1. `appsettings.json` (base)
2. `appsettings.local.json` (local secrets — not committed)
3. Environment variables (override)

Each capability has its own resource settings (endpoint, API key, deployment name, optional API version), because each may be provisioned as a separate Azure resource:

```json
{
  "AzureOpenAI": {
	"ChatCompletion":   { "Endpoint": "", "ApiKey": "", "DeploymentName": "", "ApiVersion": "" },
	"Embedding":        { "Endpoint": "", "ApiKey": "", "DeploymentName": "" },
	"Whisper":          { "Endpoint": "", "ApiKey": "", "DeploymentName": "" },
	"ImageGeneration":  { "Endpoint": "", "ApiKey": "", "DeploymentName": "" },
	"FunctionCalling":  { "Endpoint": "", "ApiKey": "", "DeploymentName": "" }
  }
}
```

- `AzureOpenAIClientFactory` builds an authenticated `AzureOpenAIClient` (with endpoint/key validation).
- `AzureOpenAIConfiguration.Load()` binds settings from the sources above.

> Put your real endpoints and keys in `appsettings.local.json` (git-ignored). Never commit secrets.

---

## Testing

- **NUnit** test suite covering every service plus configuration validation.
- Integration tests that call the live Azure OpenAI service are marked `[Explicit]`, so they only run on demand with a valid API key configured in `appsettings.local.json`.

Run the tests:

```powershell
dotnet test Azure.OpenAI\Azure.OpenAI.slnx
```

---

## Getting Started

1. Clone the repository.
2. Create `Azure.OpenAI\Azure.OpenAI.Test\appsettings.local.json` with your Azure OpenAI endpoints, keys, and deployment names.
3. Build the solution:
   ```powershell
   dotnet build Azure.OpenAI\Azure.OpenAI.slnx
   ```
4. Run the tests (integration tests require valid credentials).

---

## Skills Demonstrated

- Integrating LLMs into .NET: chat, streaming, and function/tool calling.
- Embeddings and vector similarity (cosine similarity) for semantic comparison.
- Image generation and audio transcription with Azure OpenAI.
- SOLID, interface-driven design with testable service abstractions.
- Secure, layered configuration and secrets management.
- Automated testing with NUnit on modern .NET (10) in Visual Studio 2026.
