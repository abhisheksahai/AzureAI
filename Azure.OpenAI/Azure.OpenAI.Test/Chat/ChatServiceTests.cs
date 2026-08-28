using Azure.AI.OpenAI;
using Azure.OpenAI.Chat;
using Azure.OpenAI.Configuration;
using OpenAI.Chat;

namespace Azure.OpenAI.Test
{
	public class ChatServiceTests
	{
		private AzureOpenAISettings _settings = null!;

		[SetUp]
		public void Setup()
		{
			_settings = AzureOpenAIConfiguration.Load();
		}

		[Test]
		public void Constructor_NullClient_Throws()
		{
			Assert.Throws<ArgumentNullException>(() => new ChatService(null!, _settings.ChatCompletion.DeploymentName));
		}

		[Test]
		public void Constructor_EmptyDeploymentName_Throws()
		{
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(_settings.ChatCompletion);

			Assert.Throws<ArgumentException>(() => new ChatService(client, string.Empty));
		}

		[Test]
		public void DefaultOptions_HasExpectedValues()
		{
			ChatCompletionOptions options = ChatService.DefaultOptions;

			Assert.That(options.Temperature, Is.EqualTo(0.7f));
			Assert.That(options.MaxOutputTokenCount, Is.EqualTo(1000));
		}

		[Test]
		[Explicit("Integration test - requires a valid Azure OpenAI API key in appsettings.local.json.")]
		public void CompleteChat_ReturnsResponse()
		{
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(_settings.ChatCompletion);
			IChatService chatService = new ChatService(client, _settings.ChatCompletion.DeploymentName);

			List<ChatMessage> messages = new()
			{
				new SystemChatMessage("You are a helpful assistant."),
				new UserChatMessage("Say hello in one word."),
			};

			string response = chatService.CompleteChat(messages);

			Assert.That(response, Is.Not.Empty);
		}

		[Test]
		[Explicit("Integration test - requires a valid Azure OpenAI API key in appsettings.local.json.")]
		public void CompleteChat_ParisTravelTips_ReturnsResponse()
		{
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(_settings.ChatCompletion);
			IChatService chatService = new ChatService(client, _settings.ChatCompletion.DeploymentName);

			List<ChatMessage> messages = new()
			{
				new SystemChatMessage("You are a helpful assistant."),
				new UserChatMessage("I am going to Paris, what should I see?"),
			};

			ChatCompletionOptions options = new()
			{
				MaxOutputTokenCount = 13107,
				Temperature = 1.0f,
				TopP = 1.0f,
				FrequencyPenalty = 0.0f,
				PresencePenalty = 0.0f,
			};

			string response = chatService.CompleteChat(messages, options);

			TestContext.Out.WriteLine(response);

			Assert.That(response, Is.Not.Empty);
		}

	}
}