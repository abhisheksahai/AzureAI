using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;
using System.Text.Json.Serialization;

var endPoint = "https://azureopenai-maf-resource.services.ai.azure.com";
var model = "gpt-5.5";

AIAgent meetingAgent = new AzureOpenAIClient(new Uri(endPoint), new AzureCliCredential()).GetChatClient(model).AsAIAgent(name: "MeetingAnalyst", description: "You are an AI agent extract the Topic,Action Items and overall Sentiment from the provided transcript");

string transcript = @"
[00:00] John: Good morning everyone, let's get started with our weekly meeting. 
[00:05] Sarah: Morning John, I have a few updates on the project. 
[00:10] John: Great, please go ahead. 
[00:15] Sarah: We have completed the initial design phase and are moving into development. 
[00:20] John: That's excellent news. What are the next steps? 
[00:25] Sarah: We need to finalize the requirements and start coding by next week. 
[00:30] John: Alright, let's make sure we stay on track. Any blockers? 
[00:35] Sarah: No major blockers at the moment, but we need to ensure we have all resources allocated. 
[00:40] John: Understood, let's keep communication open and address any issues promptly. 
[00:45] Sarah: Will do, thanks John. 
[00:50] John: Thanks everyone for your hard work, let's reconvene next week with updates.";
Console.WriteLine($"Analyzing meeting transcript...{transcript}");

AgentResponse<MeetingAnalysis> response = await meetingAgent.RunAsync<MeetingAnalysis>(transcript);
MeetingAnalysis? analysis = response.Result;
if (analysis != null)
{
	Console.WriteLine($"---------------------------");
	Console.WriteLine($"Full analysis : {analysis}");
	Console.WriteLine($"Topic : {analysis.Topic}");
	Console.WriteLine($"Sentiment : {analysis.Sentiment}");
	Console.WriteLine($"Action Items Length : {analysis.ActionItems.Length}");
	foreach (var actionItem in analysis.ActionItems)
	{
		Console.WriteLine($"-{actionItem}");
	}
}


public record MeetingAnalysis(
[property: JsonPropertyName("topic")] string Topic,
[property: JsonPropertyName("actionItems")] string[] ActionItems,
[property: JsonPropertyName("sentiment")] string Sentiment
);