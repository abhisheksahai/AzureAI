using Azure.AI.OpenAI;
using Azure.Identity;
using MAF.FuntionCall;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;

var endPoint = "https://azureopenai-maf-resource.services.ai.azure.com";
var model = "gpt-5.5";

AIAgent aIAgent = new AzureOpenAIClient(new Uri(endPoint), new AzureCliCredential()).GetChatClient(model).AsAIAgent(name: "LogisticSupport", instructions: "You are a logistic support agent. Provide the current status of a specific logistic order based on the provided order ID.", tools: [AIFunctionFactory.Create(LogisticTools.GetLogisticStatus)]);

Console.WriteLine($"Agent {aIAgent.Name} initialised");

string promt1 = "what is the status of ORD-12345";
Console.WriteLine($"User : {promt1}");
AgentResponse agentResponse = await aIAgent.RunAsync(promt1);
Console.WriteLine($"Agent : {agentResponse.Text}");

string promt2 = $"What is the status of ORD-11111";
Console.WriteLine($"User : {promt2}");
Console.Write($"Agent : ");
await foreach (AgentResponseUpdate agentResponseUpdate in aIAgent.RunStreamingAsync(promt2))
{
	Console.Write($"{agentResponseUpdate.Text}");
}
Console.WriteLine();