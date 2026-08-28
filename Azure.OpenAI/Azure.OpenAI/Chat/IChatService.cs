using OpenAI.Chat;

namespace Azure.OpenAI.Chat
{
	public interface IChatService
	{
		string CompleteChat(IEnumerable<ChatMessage> messages, ChatCompletionOptions? options = null);

		IEnumerable<string> CompleteChatStreaming(IEnumerable<ChatMessage> messages, ChatCompletionOptions? options = null);
	}
}
