using System.Text;
using Azure.AI.OpenAI;
using OpenAI.Chat;

namespace Azure.OpenAI.Chat
{
	public class ChatService : IChatService
	{
		private readonly ChatClient _chatClient;

		public ChatService(AzureOpenAIClient client, string deploymentName)
		{
			ArgumentNullException.ThrowIfNull(client);

			if (string.IsNullOrWhiteSpace(deploymentName))
			{
				throw new ArgumentException("Deployment name is required.", nameof(deploymentName));
			}

			_chatClient = client.GetChatClient(deploymentName);
		}

		public static ChatCompletionOptions DefaultOptions => new()
		{
			Temperature = 0.7f,
			MaxOutputTokenCount = 1000,
			TopP = 0.95f,
			FrequencyPenalty = 0.0f,
			PresencePenalty = 0.0f,
		};

		public string CompleteChat(IEnumerable<ChatMessage> messages, ChatCompletionOptions? options = null)
		{
			ArgumentNullException.ThrowIfNull(messages);

			ChatCompletion completion = _chatClient.CompleteChat(messages, options ?? DefaultOptions);

			StringBuilder builder = new();
			foreach (ChatMessageContentPart part in completion.Content)
			{
				builder.Append(part.Text);
			}

			return builder.ToString();
		}

		public IEnumerable<string> CompleteChatStreaming(IEnumerable<ChatMessage> messages, ChatCompletionOptions? options = null)
		{
			ArgumentNullException.ThrowIfNull(messages);

			foreach (StreamingChatCompletionUpdate update in _chatClient.CompleteChatStreaming(messages, options ?? DefaultOptions))
			{
				foreach (ChatMessageContentPart part in update.ContentUpdate)
				{
					yield return part.Text;
				}
			}
		}
	}
}
