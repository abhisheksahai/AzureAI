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

			return new AzureOpenAIClient(
				new Uri(settings.Endpoint),
				new AzureKeyCredential(settings.ApiKey));
		}
	}
}
