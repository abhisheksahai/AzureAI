namespace Azure.OpenAI.Configuration
{
	/// <summary>
	/// Root configuration for Azure OpenAI. Each resource (Chat Completion, Embedding, ...)
	/// has its own endpoint, API key and deployment name because they may be provisioned
	/// as separate Azure resources with independent keys.
	/// </summary>
	public class AzureOpenAISettings
	{
		public const string SectionName = "AzureOpenAI";

		public AzureOpenAIResourceSettings ChatCompletion { get; set; } = new();

		public AzureOpenAIResourceSettings Embedding { get; set; } = new();
	}

	/// <summary>
	/// Connection settings for a single Azure OpenAI resource/deployment.
	/// </summary>
	public class AzureOpenAIResourceSettings
	{
		public string Endpoint { get; set; } = string.Empty;

		public string ApiKey { get; set; } = string.Empty;

		public string DeploymentName { get; set; } = string.Empty;

		/// <summary>
		/// Azure OpenAI REST API version, e.g. "2024-10-21".
		/// Optional - when empty the SDK's default service version is used.
		/// Supported values depend on the installed Azure.AI.OpenAI SDK version.
		/// </summary>
		public string ApiVersion { get; set; } = string.Empty;
	}
}
