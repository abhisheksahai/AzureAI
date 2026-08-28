using Azure.AI.OpenAI;
using Microsoft.Extensions.Configuration;

namespace Azure.OpenAI.Configuration
{
	public static class AzureOpenAIConfiguration
	{
		public static AzureOpenAISettings Load(string? basePath = null)
		{
			IConfiguration configuration = new ConfigurationBuilder()
				.SetBasePath(basePath ?? AppContext.BaseDirectory)
				.AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
				.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: false)
				.AddEnvironmentVariables()
				.Build();

			AzureOpenAISettings settings = new();
			configuration.GetSection(AzureOpenAISettings.SectionName).Bind(settings);
			return settings;
		}
	}

	public static class AzureOpenAIClientFactory
	{
		public static AzureOpenAIClient Create(AzureOpenAIResourceSettings settings)
		{
			ArgumentNullException.ThrowIfNull(settings);

			if (string.IsNullOrWhiteSpace(settings.Endpoint))
			{
				throw new InvalidOperationException("Azure OpenAI endpoint is not configured.");
			}

			if (string.IsNullOrWhiteSpace(settings.ApiKey))
			{
				throw new InvalidOperationException("Azure OpenAI API key is not configured.");
			}

			AzureOpenAIClientOptions options = new();
			if (!string.IsNullOrWhiteSpace(settings.ApiVersion))
			{
				options = new AzureOpenAIClientOptions(ParseServiceVersion(settings.ApiVersion));
			}

			return new AzureOpenAIClient(
				new Uri(settings.Endpoint),
				new AzureKeyCredential(settings.ApiKey),
				options);
		}

		/// <summary>
		/// Maps an Azure OpenAI REST api-version string (e.g. "2025-01-01-preview")
		/// to the SDK's <see cref="AzureOpenAIClientOptions.ServiceVersion"/> enum.
		/// </summary>
		private static AzureOpenAIClientOptions.ServiceVersion ParseServiceVersion(string apiVersion)
		{
			// "2025-01-01-preview" -> "V2025_01_01_preview" (matched case-insensitively).
			string enumName = "V" + apiVersion.Replace("-", "_");

			if (Enum.TryParse(enumName, ignoreCase: true, out AzureOpenAIClientOptions.ServiceVersion version))
			{
				return version;
			}

			throw new InvalidOperationException(
				$"Unsupported Azure OpenAI api-version '{apiVersion}'. Expected a value like '2025-01-01-preview' that maps to a known ServiceVersion.");
		}
	}
}
