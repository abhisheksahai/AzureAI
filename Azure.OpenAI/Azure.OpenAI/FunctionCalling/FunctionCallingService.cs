using System.Text;
using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace Azure.OpenAI.FunctionCalling
{
	public class FunctionCallingService : IFunctionCallingService
	{
		private const int MaxToolIterations = 5;

		private readonly ChatClient _chatClient;

		public FunctionCallingService(AzureOpenAIClient client, string deploymentName)
		{
			ArgumentNullException.ThrowIfNull(client);

			if (string.IsNullOrWhiteSpace(deploymentName))
			{
				throw new ArgumentException("Deployment name is required.", nameof(deploymentName));
			}

			_chatClient = client.GetChatClient(deploymentName);
		}

		public string CompleteWithTools(
			IEnumerable<ChatMessage> messages,
			IEnumerable<ChatTool> tools,
			Func<string, string, string> toolInvoker,
			ChatCompletionOptions? options = null)
		{
			ArgumentNullException.ThrowIfNull(messages);
			ArgumentNullException.ThrowIfNull(tools);
			ArgumentNullException.ThrowIfNull(toolInvoker);

			List<ChatMessage> conversation = messages.ToList();

			ChatCompletionOptions completionOptions = options ?? new ChatCompletionOptions();
			foreach (ChatTool tool in tools)
			{
				completionOptions.Tools.Add(tool);
			}

			for (int iteration = 0; iteration < MaxToolIterations; iteration++)
			{
				ChatCompletion completion = _chatClient.CompleteChat(conversation, completionOptions);

				if (completion.FinishReason != ChatFinishReason.ToolCalls)
				{
					return BuildText(completion);
				}

				// Add the assistant message that carries the tool call requests.
				conversation.Add(new AssistantChatMessage(completion));

				foreach (ChatToolCall toolCall in completion.ToolCalls)
				{
					string result = toolInvoker(toolCall.FunctionName, toolCall.FunctionArguments.ToString());
					conversation.Add(new ToolChatMessage(toolCall.Id, result));
				}
			}

			throw new InvalidOperationException(
				$"Model did not produce a final answer within {MaxToolIterations} tool-call iterations.");
		}

		private static string BuildText(ChatCompletion completion)
		{
			StringBuilder builder = new();
			foreach (ChatMessageContentPart part in completion.Content)
			{
				builder.Append(part.Text);
			}

			return builder.ToString();
		}
	}
}
