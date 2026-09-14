using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using MAF.HITL.ApproveRequiredFunction;
using System.Text.Json;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

var endPoint = "https://azureopenai-maf-resource.services.ai.azure.com";
var model = "gpt-5.5";

AIFunction aIFunction = AIFunctionFactory.Create(FinanceTools.IssueRefund);
AIFunction securedRefund = new ApprovalRequiredAIFunction(aIFunction);

AIAgent agent = new AzureOpenAIClient(new Uri(endPoint), new AzureCliCredential()).GetChatClient(model).AsAIAgent(name: "FinanceSupport", instructions: "You are a customer support agent specializing in finance-related inquiries.", tools: [securedRefund]);

AgentSession session = await agent.CreateSessionAsync();

Console.WriteLine($"Agent {agent.Name} initialised");
string userPromt = "I was charged two times. Please issue a refund for order ORD-12345 with an amount of 100.00";
Console.WriteLine($"User : {userPromt}");

AgentResponse response = await agent.RunAsync(userPromt, session);

var approvalRequest = response.Messages.SelectMany(x => x.Contents).OfType<ToolApprovalRequestContent>().ToList();
if (approvalRequest.Any())
{
	ToolApprovalRequestContent request = approvalRequest.First();

	var requestTool = (FunctionCallContent)request.ToolCall;
	string toolName = requestTool.Name;
	string toolArgs = JsonSerializer.Serialize(requestTool.Arguments);

	Console.ForegroundColor = ConsoleColor.Yellow;
	Console.WriteLine($"Approval request for tool '{toolName}' with arguments: {toolArgs}");
	Console.WriteLine("Please approve or deny the request (type 'Y' or 'N'):");
	Console.ResetColor();

	string? userInput = Console.ReadLine();
	bool isApproved = userInput?.Trim().ToUpper() == "Y";

	var approvalMessage = new ChatMessage(ChatRole.User, new[] { request.CreateResponse(isApproved) });

	response = await agent.RunAsync(approvalMessage, session);

	Console.WriteLine($"Agent : {response.Text}");
}