using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

var endPoint = "https://azureopenai-maf-resource.services.ai.azure.com";
var model = "gpt-5.5";

IChatClient chatClient = new AzureOpenAIClient(new Uri(endPoint), new AzureCliCredential()).GetChatClient(model).AsIChatClient();

AIAgent supportAgent = chatClient.AsAIAgent(name: "NetworkSupport", instructions: "You are tier1 IT support agent your answers must be consise,professional and limited");

Console.WriteLine($"Agent {supportAgent.Name} is online");

string useQuery = "We have enabled WAF in azure frontdoor/appservice one of the post api is getting blocked as its payload is json which has complex object, could you help";

await foreach (AgentResponseUpdate update in supportAgent.RunStreamingAsync(useQuery))
{
	Console.Write(update.Text);
}