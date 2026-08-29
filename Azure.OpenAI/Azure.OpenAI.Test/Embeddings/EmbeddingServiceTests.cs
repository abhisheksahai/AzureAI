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
			const string secondText = "Hot";

			IReadOnlyList<ReadOnlyMemory<float>> embeddings =
				service.GenerateEmbeddings(new[] { firstText, secondText });

			double similarity = VectorMath.CosineSimilarity(embeddings[0], embeddings[1]);

			TestContext.Out.WriteLine($"'{firstText}' vs '{secondText}'");
			TestContext.Out.WriteLine($"Cosine similarity: {similarity:F6}");
			TestContext.Out.WriteLine($"Similarity: {similarity * 100:F4} %");

			Assert.That(similarity, Is.InRange(-1.0, 1.0));
		}


		[Test]
		[TestCase("This study explores groundbreaking advancements in renewable energy technologies, focusing on solar and wind power's efficiency improvements. By analyzing recent developments, we highlight the potential for these technologies to significantly reduce global dependency on fossil fuels, thereby mitigating climate change impacts.", ExpectedResult = false)]
		[TestCase("Artificial Intelligence (AI) holds transformative potential for environmental protection, offering tools for better predicting climate change patterns and optimizing resource use. This paper examines AI applications in monitoring environmental degradation and managing natural resources more efficiently, presenting a case for integrating AI strategies into conservation efforts.", ExpectedResult = true)]
		[TestCase("Marine biodiversity faces significant threats from climate change, with rising temperatures and acidification leading to coral bleaching and loss of habitat. This research analyzes the consequences of these changes on marine ecosystems and emphasizes the urgency of adopting conservation strategies to protect marine life.", ExpectedResult = false)]
		public bool PlagiarismDetectionTest(string inputText)
		{
			string suspectedPlagiarizedText = "Recent advancements in solar and wind energy technologies have shown promising potential to lessen the world's reliance on non-renewable energy sources, thus playing a crucial role in combating climate change. Furthermore, the utilization of Artificial Intelligence offers unparalleled opportunities in the realm of environmental conservation, aiding in the accurate prediction of climatic trends and the efficient management of ecological resources. Additionally, the adverse effects of climate change on ocean life, particularly through the phenomenon of coral bleaching, underscore the need for immediate action to safeguard marine ecosystems.";
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(_settings.Embedding);
			IEmbeddingService service = new EmbeddingService(client, _settings.Embedding.DeploymentName);

			IReadOnlyList<ReadOnlyMemory<float>> embeddings = service.GenerateEmbeddings(new[] { inputText, suspectedPlagiarizedText });
			double similarity = VectorMath.CosineSimilarity(embeddings[0], embeddings[1]);
			return similarity > 0.9;
		}

	}
}
