using Azure.AI.OpenAI;
using Azure.OpenAI.Configuration;
using Azure.OpenAI.Embeddings;

namespace Azure.OpenAI.Test
{
	public class EmbeddingServiceTests
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
			Assert.Throws<ArgumentNullException>(() => new EmbeddingService(null!, "text-embedding-3-small"));
		}

		[Test]
		public void Constructor_EmptyDeploymentName_Throws()
		{
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(new AzureOpenAISettings
			{
				Endpoint = "https://example.openai.azure.com/",
				ApiKey = "key",
			});

			Assert.Throws<ArgumentException>(() => new EmbeddingService(client, string.Empty));
		}

		[Test]
		public void GenerateEmbedding_EmptyInput_Throws()
		{
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(new AzureOpenAISettings
			{
				Endpoint = "https://example.openai.azure.com/",
				ApiKey = "key",
			});
			IEmbeddingService service = new EmbeddingService(client, "text-embedding-3-small");

			Assert.Throws<ArgumentException>(() => service.GenerateEmbedding(string.Empty));
		}

		[Test]
		[Explicit("Integration test - requires a valid Azure OpenAI API key in appsettings.json.")]
		public void GenerateEmbedding_ReturnsVector()
		{
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(_settings);
			IEmbeddingService service = new EmbeddingService(client, _settings.EmbeddingDeploymentName);

			ReadOnlyMemory<float> vector = service.GenerateEmbedding("Azure OpenAI embeddings");

			Assert.That(vector.Length, Is.GreaterThan(0));
		}
	}
}
