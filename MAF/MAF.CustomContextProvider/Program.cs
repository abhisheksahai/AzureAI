using Azure.AI.OpenAI;
using Azure.Identity;
using MAF.CustomContextProvider;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using System.Text;
using System.Text.Json;

var endPoint = "https://azureopenai-maf-resource.services.ai.azure.com";
var model = "gpt-5.5";

ChatClient chatClient = new AzureOpenAIClient(new Uri(endPoint), new AzureCliCredential()).GetChatClient(model);

AIAgent agent = chatClient.AsAIAgent
(new ChatClientAgentOptions()
{
	Name = "CorporateGuide",
	ChatOptions = new ChatOptions() { Instructions = "You are a friendly corporate guide" },
	AIContextProviders = new AIContextProvider[] { new EmployeeContextProvider(chatClient.AsIChatClient()) }
});

AgentSession session = await agent.CreateSessionAsync();
Console.WriteLine("Session created successfully.");
Console.WriteLine(await agent.RunAsync("What is the corporate policy on remote work",session));
Console.WriteLine(await agent.RunAsync("My name is Abhishek i work with IT Department", session));

JsonElement serialisedSession = await agent.SerializeSessionAsync(session);
Console.WriteLine("Session serialized successfully.");

var resumedSession = await agent.DeserializeSessionAsync(serialisedSession);
Console.WriteLine(await agent.RunAsync("Can you remind me of my department", resumedSession));

var profileProvider = agent.GetService<EmployeeContextProvider>();
var profile = profileProvider?.GetProfile(resumedSession);
Console.WriteLine($"Employee Name: {profile?.EmployeeName}, Department: {profile?.Department}");

internal sealed class EmployeeContextProvider : AIContextProvider
{
	private readonly ProviderSessionState<EmployeeProfile> _sessionState;
	private readonly IChatClient _chatClient;

	public EmployeeContextProvider(IChatClient chatClient) : base(null, null)
	{

		_sessionState = new ProviderSessionState<EmployeeProfile>
			(
				_ => new EmployeeProfile(),
				this.GetType().Name
			);
		_chatClient = chatClient;
	}

	public override IReadOnlyList<string> StateKeys => [_sessionState.StateKey];

	public EmployeeProfile GetProfile(AgentSession session) => _sessionState.GetOrInitializeState(session);

	protected override ValueTask<AIContext> ProvideAIContextAsync(InvokingContext invokingContext, CancellationToken cancellationToken = default)
	{
		var profile = _sessionState.GetOrInitializeState(invokingContext.Session);
		StringBuilder instruction = new();
		instruction.AppendLine
		(profile.EmployeeName is null ? "Ask the user for their name and politely decline to answer corporate question until name is provided" : $"The user name is {profile.EmployeeName}")
		.AppendLine(profile.Department is null ? "Ask the user for their department and politely decline to answer corporate question until department is provided" : $"The user department is {profile.Department}");

		return new ValueTask<AIContext>(new AIContext() { Instructions = instruction.ToString() });
	}

	protected override async ValueTask StoreAIContextAsync(InvokedContext invokedContext, CancellationToken cancellationToken = default)
	{
		var profile = _sessionState.GetOrInitializeState(invokedContext.Session);
		if ((profile.EmployeeName is null || profile.Department is null) && invokedContext.RequestMessages.Any())
		{
			var result = await _chatClient.GetResponseAsync<EmployeeProfile>
			(invokedContext.RequestMessages, new ChatOptions() { Instructions = "Extract username and department from the user's response" }, cancellationToken: cancellationToken);

			profile.EmployeeName ??= result.Result?.EmployeeName;
			profile.Department ??= result.Result?.Department;
		}
		_sessionState.SaveState(invokedContext.Session, profile);
	}

}