namespace Azure.OpenAI.Configuration
{
	public class AzureOpenAISettings
	{
		public const string SectionName = "AzureOpenAI";

		public string Endpoint { get; set; } = string.Empty;

		public string ApiKey { get; set; } = string.Empty;

		public string ChatDeploymentName { get; set; } = string.Empty;

		public string EmbeddingDeploymentName { get; set; } = string.Empty;
	}
}
