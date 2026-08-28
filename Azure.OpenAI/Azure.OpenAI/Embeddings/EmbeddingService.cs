using Azure.AI.OpenAI;
using OpenAI.Embeddings;

namespace Azure.OpenAI.Embeddings
{
	public class EmbeddingService : IEmbeddingService
	{
		private readonly EmbeddingClient _embeddingClient;

		public EmbeddingService(AzureOpenAIClient client, string deploymentName)
		{
			ArgumentNullException.ThrowIfNull(client);

			if (string.IsNullOrWhiteSpace(deploymentName))
			{
				throw new ArgumentException("Deployment name is required.", nameof(deploymentName));
			}

			_embeddingClient = client.GetEmbeddingClient(deploymentName);
		}

		public ReadOnlyMemory<float> GenerateEmbedding(string input)
		{
			if (string.IsNullOrWhiteSpace(input))
			{
				throw new ArgumentException("Input text is required.", nameof(input));
			}

			OpenAIEmbedding embedding = _embeddingClient.GenerateEmbedding(input);
			return embedding.ToFloats();
		}

		public IReadOnlyList<ReadOnlyMemory<float>> GenerateEmbeddings(IEnumerable<string> inputs)
		{
			ArgumentNullException.ThrowIfNull(inputs);

			List<string> inputList = inputs.ToList();
			if (inputList.Count == 0)
			{
				throw new ArgumentException("At least one input text is required.", nameof(inputs));
			}

			OpenAIEmbeddingCollection collection = _embeddingClient.GenerateEmbeddings(inputList);
			return collection.Select(e => e.ToFloats()).ToList();
		}
	}
}
