namespace Azure.OpenAI.Embeddings
{
	/// <summary>
	/// Vector math helpers for comparing embeddings.
	/// </summary>
	public static class VectorMath
	{
		/// <summary>
		/// Computes the cosine similarity between two vectors.
		/// Result is in the range [-1, 1]; 1 means identical direction (most similar).
		/// Equivalent to numpy: dot(a, b) / (norm(a) * norm(b)).
		/// </summary>
		public static double CosineSimilarity(ReadOnlyMemory<float> first, ReadOnlyMemory<float> second)
		{
			if (first.Length != second.Length)
			{
				throw new ArgumentException("Vectors must have the same length.");
			}

			if (first.Length == 0)
			{
				throw new ArgumentException("Vectors must not be empty.");
			}

			ReadOnlySpan<float> a = first.Span;
			ReadOnlySpan<float> b = second.Span;

			double dotProduct = 0d;
			double magnitudeA = 0d;
			double magnitudeB = 0d;

			for (int i = 0; i < a.Length; i++)
			{
				dotProduct += a[i] * b[i];
				magnitudeA += a[i] * a[i];
				magnitudeB += b[i] * b[i];
			}

			if (magnitudeA == 0d || magnitudeB == 0d)
			{
				throw new ArgumentException("Vectors must not be zero vectors.");
			}

			return dotProduct / (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
		}
	}
}
