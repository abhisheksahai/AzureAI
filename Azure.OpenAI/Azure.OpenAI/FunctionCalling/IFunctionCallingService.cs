using OpenAI.Chat;

namespace Azure.OpenAI.FunctionCalling
{
	public interface IFunctionCallingService
	{
		/// <summary>
		/// Runs a chat completion that may invoke the supplied tools. When the model requests
		/// a tool call, <paramref name="toolInvoker"/> is called to produce the tool result and
		/// the conversation continues until the model returns a final text answer.
		/// </summary>
		/// <param name="messages">The initial conversation messages.</param>
		/// <param name="tools">The tool definitions exposed to the model.</param>
		/// <param name="toolInvoker">Callback that executes a requested tool and returns its result as a string.</param>
		/// <param name="options">Optional completion options.</param>
		/// <returns>The final assistant text response.</returns>
		string CompleteWithTools(
			IEnumerable<ChatMessage> messages,
			IEnumerable<ChatTool> tools,
			Func<string, string, string> toolInvoker,
			ChatCompletionOptions? options = null);
	}
}
