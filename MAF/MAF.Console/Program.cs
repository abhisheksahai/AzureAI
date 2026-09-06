using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;

var endPoint = "https://azureopenai-maf-resource.services.ai.azure.com";
var model = "gpt-5.5";

AIAgent agent = new AzureOpenAIClient(new Uri(endPoint), new AzureCliCredential()).GetChatClient(model).AsAIAgent(instructions: "you are a friendly assistant. keep your answers brief");
var response = await agent.RunAsync("Tell me about Royal Enfield Classic 350 - Stelth Black");
Console.WriteLine(response);