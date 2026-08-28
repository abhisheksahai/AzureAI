namespace Azure.OpenAI.Embeddings
{
	public interface IEmbeddingService
	{
		ReadOnlyMemory<float> GenerateEmbedding(string input);

		IReadOnlyList<ReadOnlyMemory<float>> GenerateEmbeddings(IEnumerable<string> inputs);
	}
}
