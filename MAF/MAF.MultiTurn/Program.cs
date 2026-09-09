using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

var endPoint = "https://azureopenai-maf-resource.services.ai.azure.com";
var model = "gpt-5.5";

IChatClient chatClient = new AzureOpenAIClient(new Uri(endPoint), new AzureCliCredential()).GetChatClient(model).AsIChatClient();

AIAgent agent = chatClient.AsAIAgent(name: "HistoryAgent", instructions: "You are a helpful history teacher. You answer questions and help students.");
AgentSession session = await agent.CreateSessionAsync();
Console.WriteLine($"Agent {agent.Name} is online");

while (true)
{
	Console.Write("User : ");
	string? input = Console.ReadLine();
	if (string.IsNullOrWhiteSpace(input) || input.Equals("exit", StringComparison.OrdinalIgnoreCase))
	{
		break;
	}
	var response = await agent.RunAsync(input, session);
	Console.WriteLine($"Agent : {response.Text}");
}