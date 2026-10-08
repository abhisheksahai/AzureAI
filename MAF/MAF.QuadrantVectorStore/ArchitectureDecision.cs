using Microsoft.Extensions.VectorData;

namespace MAF.QuadrantVectorStore
{
	public record ArchitectureDecision
	{
		[VectorStoreKey]
		public Guid DocumentId { get; set; } = Guid.NewGuid();

		[VectorStoreData]
		public string Title { get; set; } = string.Empty;

		[VectorStoreData]
		public string Content { get; set; } = string.Empty;

		[VectorStoreVector(1536)]
		public ReadOnlyMemory<float> ContentVector { get; set; }
	}
}