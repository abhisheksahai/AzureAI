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