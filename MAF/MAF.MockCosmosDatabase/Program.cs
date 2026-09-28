using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using OpenAI.Chat;
using System.Collections.Concurrent;
using System.Text.Json;

Console.WriteLine("Starting Stateless Agent Service...");
var repository = new MockCosmosDatabaseRepository();
var agentService = new StatelessAgentService(repository);

string userId = "user123";
string response1 = await agentService.HandleUserMessageAsync(userId, "Hello! I am planning for a trip to Bhimashankar jyotirlinga");
Console.WriteLine($"Response 1: {response1}");
string response2 = await agentService.HandleUserMessageAsync(userId, "Do you remember my previous message? Can you suggest some good hotels nearby?");
Console.WriteLine($"Response 2: {response2}");


public class StatelessAgentService
{
	string endPoint = "https://azureopenai-maf-resource.services.ai.azure.com";
	string model = "gpt-5.5";

	private AIAgent aIAgent = null;
	private ISessionRepository _sessionRepository;
	public StatelessAgentService(ISessionRepository sessionRepository)
	{
		_sessionRepository = sessionRepository;
		aIAgent = new AzureOpenAIClient(new Uri(endPoint), new AzureCliCredential())
			.GetChatClient(model)
			.AsAIAgent(name: "PersistenceGuide", instructions: "you are a friendly assistant. keep your answers brief. You remember details over long period of time");
	}

	public async Task<string> HandleUserMessageAsync(string sessionId, string userMessage)
	{
		AgentSession agentSession;

		string? savedSessionJson = await _sessionRepository.GetSessionJsonAsync(sessionId);
		if (!string.IsNullOrWhiteSpace(savedSessionJson))
		{
			using JsonDocument document = JsonDocument.Parse(savedSessionJson);
			agentSession = await aIAgent.DeserializeSessionAsync(document.RootElement);
			Console.WriteLine($"Restored session for sessionId: {sessionId}");
		}
		else
		{
			agentSession = await aIAgent.CreateSessionAsync();
			Console.WriteLine($"Created new session for sessionId: {sessionId}");
		}

		AgentResponse response = await aIAgent.RunAsync(userMessage, agentSession);

		JsonElement jsonElement = await aIAgent.SerializeSessionAsync(agentSession);
		string updatedSessionJson = JsonSerializer.Serialize(jsonElement);
		await _sessionRepository.SaveSessionJsonAsync(sessionId, updatedSessionJson);
		return response.Text;
	}

}

public class MockCosmosDatabaseRepository : ISessionRepository
{
	private readonly ConcurrentDictionary<string, string> _dataStore = new();

	public Task<string?> GetSessionJsonAsync(string sessionId)
	{
		_dataStore.TryGetValue(sessionId, out var json);
		return Task.FromResult(json ?? null);
	}

	public Task SaveSessionJsonAsync(string sessionId, string jsonPayload)
	{
		_dataStore[sessionId] = jsonPayload;
		return Task.CompletedTask;
	}
}

public interface ISessionRepository
{
	Task<string?> GetSessionJsonAsync(string sessionId);
	Task SaveSessionJsonAsync(string sessionId, string jsonPayload);
}