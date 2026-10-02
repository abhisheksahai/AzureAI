using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;

var endPoint = "https://azureopenai-maf-resource.services.ai.azure.com";
var model = "gpt-5.5";

IChatClient chatClient = new AzureOpenAIClient(new Uri(endPoint), new AzureCliCredential()).GetChatClient(model).AsIChatClient();

AIAgent triageAgent = chatClient.AsAIAgent(
	name: "Triage",
	instructions: "Analyse the user IT request and categorize it strictly as either 'Hardware' of 'Software'"
);

AIAgent hardwareAgent = chatClient.AsAIAgent(
	name: "Hardware",
	instructions: "You are a hardware expert. You will receive user requests categorized as 'Hardware' and provide detailed analysis and solutions."
);

AIAgent softwareAgent = chatClient.AsAIAgent(
	name: "Software",
	instructions: "You are a software expert. You will receive user requests categorized as 'Software' and provide detailed analysis and solutions."
);


Func<TicketState, TicketState> triageFunc = state =>
{
	Console.WriteLine($"Triage Agent received user query: {state.UserQuery}");
	AgentResponse triageAgentResponse = triageAgent.RunAsync(state.UserQuery).GetAwaiter().GetResult();
	string category = triageAgentResponse.Text.Trim();
	Console.WriteLine($"Triage Agent categorized the request as: {category}");
	return state with { Category = category };
};
var triageNode = triageFunc.BindAsExecutor("TriageNode");

Func<TicketState, TicketState> hardwareFunc = state =>
{
	Console.WriteLine($"Hardware Agent received user query: {state.UserQuery}");
	AgentResponse hardwareResponse = hardwareAgent.RunAsync(state.UserQuery).GetAwaiter().GetResult();
	return state with { Resolution = hardwareResponse.Text };
};
var hardwareNode = hardwareFunc.BindAsExecutor("HardwareNode");

Func<TicketState, TicketState> softwareFunc = state =>
{
	Console.WriteLine($"Software Agent received user query: {state.UserQuery}");
	AgentResponse softwareResponse = softwareAgent.RunAsync(state.UserQuery).GetAwaiter().GetResult();
	return state with { Resolution = softwareResponse.Text };
};
var softwareNode = softwareFunc.BindAsExecutor("SoftwareNode");

var workFlow = new WorkflowBuilder(triageNode)
			   .AddEdge<TicketState>(triageNode, hardwareNode, condition: state => state?.Category.Contains("Hardware", StringComparison.OrdinalIgnoreCase) ?? false)
			   .AddEdge<TicketState>(triageNode, softwareNode, condition: state => state?.Category.Contains("Software", StringComparison.OrdinalIgnoreCase) ?? false)
			   .Build();

Console.WriteLine("--Incoming enterprise IT ticket--");
var initialState = new TicketState(UserQuery: "My laptop is not turning on.");

await using StreamingRun streamingRun = await InProcessExecution.RunStreamingAsync(workFlow, initialState);
TicketState? finalState = null;

await foreach (WorkflowEvent evt in streamingRun.WatchStreamAsync())
{
	if (evt is ExecutorCompletedEvent executionComplete)
	{
		Console.WriteLine($"Executor {executionComplete.ExecutorId}");
		if (executionComplete.Data is TicketState state)
		{
			finalState = state;
			Console.WriteLine($"Category: {state.Category}, Resolution: {state.Resolution}");
		}
	}
}

Console.WriteLine($"Final Resolution: {finalState?.Resolution}");
Console.WriteLine("--End of ticket processing--");

public record TicketState(string UserQuery, string Category = "Unassigned", string Resolution = "");