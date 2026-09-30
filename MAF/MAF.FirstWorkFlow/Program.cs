using Microsoft.Agents.AI.Workflows;

Console.WriteLine("Hello, World!");

Func<CustomerPayload, CustomerPayload> validateFunc = payload =>
{
	Console.WriteLine($"Inspecting payload for {payload.CompanyName}");
	bool isValid = !string.IsNullOrWhiteSpace(payload.CompanyName);
	return payload with { IsValidated = isValid, Status = isValid ? "Validated" : "Rejected" };
};
var validatorExecutor = validateFunc.BindAsExecutor("ValidationNode");

Func<CustomerPayload, CustomerPayload> enrichFunc = payload =>
{
	Console.WriteLine($"Enriching payload for {payload.Industry}");
	return payload with { Status = "Enriched" };
};
var enricherExecutor = enrichFunc.BindAsExecutor("EnrichmentNode");

Func<CustomerPayload, CustomerPayload> auditFunc = payload =>
{
	Console.WriteLine($"Auditing payload for {payload.CompanyName}");
	return payload with { Status = "Audited" };
};
var auditorExecutor = auditFunc.BindAsExecutor("AuditingNode");

var workflow = new WorkflowBuilder(validatorExecutor)
			   .AddEdge<CustomerPayload>(validatorExecutor, enricherExecutor, condition: p => p?.IsValidated == true)
			   .AddEdge<CustomerPayload>(validatorExecutor, auditorExecutor, condition: p => p?.IsValidated == false)
			   .AddEdge(enricherExecutor, auditorExecutor)
			   .Build();

Console.WriteLine("Starting workflow execution...");
var initialPayload = new CustomerPayload("Acme Corp", "Manufacturing");

await using StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, initialPayload);

await foreach (WorkflowEvent evt in run.WatchStreamAsync())
{
	if (evt is ExecutorCompletedEvent executorCompleted)
	{
		Console.WriteLine($"Step: {executorCompleted.ExecutorId} completed successfully");
	}
	;
}

public record CustomerPayload(string CompanyName, string Industry, bool IsValidated = false, string Status = "New");