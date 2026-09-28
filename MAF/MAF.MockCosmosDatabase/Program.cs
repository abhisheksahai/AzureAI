using System.Collections.Concurrent;

Console.WriteLine("Hello, World!");


public interface ISessionRepository
{
	Task<string?> GetSessionJsonAsync(string sessionId);
	Task SaveSessionJsonAsync(string sessionId, string jsonPayload);
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
