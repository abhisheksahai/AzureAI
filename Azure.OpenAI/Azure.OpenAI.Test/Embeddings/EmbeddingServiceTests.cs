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
			Assert.Throws<ArgumentNullException>(() => new EmbeddingService(null!, _settings.Embedding.DeploymentName));
		}

		[Test]
		public void Constructor_EmptyDeploymentName_Throws()
		{
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(_settings.Embedding);

			Assert.Throws<ArgumentException>(() => new EmbeddingService(client, string.Empty));
		}

		[Test]
		public void GenerateEmbedding_EmptyInput_Throws()
		{
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(_settings.Embedding);
			IEmbeddingService service = new EmbeddingService(client, _settings.Embedding.DeploymentName);

			Assert.Throws<ArgumentException>(() => service.GenerateEmbedding(string.Empty));
		}

		[Test]
		[Explicit("Integration test - requires a valid Azure OpenAI API key in appsettings.local.json.")]
		public void GenerateEmbedding_ReturnsVector()
		{
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(_settings.Embedding);
			IEmbeddingService service = new EmbeddingService(client, _settings.Embedding.DeploymentName);

			ReadOnlyMemory<float> vector = service.GenerateEmbedding("Hot Cold");

			Assert.That(vector.Length, Is.GreaterThan(0));
		}

		[Test]
		public void CosineSimilarity_IdenticalVectors_ReturnsOne()
		{
			ReadOnlyMemory<float> vector = new float[] { 1f, 2f, 3f };

			double similarity = VectorMath.CosineSimilarity(vector, vector);

			Assert.That(similarity, Is.EqualTo(1.0).Within(1e-6));
		}

		[Test]
		public void CosineSimilarity_OrthogonalVectors_ReturnsZero()
		{
			ReadOnlyMemory<float> a = new float[] { 1f, 0f };
			ReadOnlyMemory<float> b = new float[] { 0f, 1f };

			double similarity = VectorMath.CosineSimilarity(a, b);

			Assert.That(similarity, Is.EqualTo(0.0).Within(1e-6));
		}

		[Test]
		[Explicit("Integration test - requires a valid Azure OpenAI API key in appsettings.local.json.")]
		public void GenerateEmbedding_SimilarityBetweenTwoPhrases()
		{
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(_settings.Embedding);
			IEmbeddingService service = new EmbeddingService(client, _settings.Embedding.DeploymentName);

			const string firstText = "Hot";
			const string secondText = "Cold";

			IReadOnlyList<ReadOnlyMemory<float>> embeddings =
				service.GenerateEmbeddings(new[] { firstText, secondText });

			double similarity = VectorMath.CosineSimilarity(embeddings[0], embeddings[1]);

			TestContext.Out.WriteLine($"'{firstText}' vs '{secondText}'");
			TestContext.Out.WriteLine($"Cosine similarity: {similarity:F6}");
			TestContext.Out.WriteLine($"Similarity: {similarity * 100:F4} %");

			Assert.That(similarity, Is.InRange(-1.0, 1.0));
		}
	}
}
