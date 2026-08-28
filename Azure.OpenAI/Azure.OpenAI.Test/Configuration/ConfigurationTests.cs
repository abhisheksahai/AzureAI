using Azure.OpenAI.Configuration;

namespace Azure.OpenAI.Test
{
	public class ConfigurationTests
	{
		[Test]
		public void Load_BindsAzureOpenAISection()
		{
			AzureOpenAISettings settings = AzureOpenAIConfiguration.Load();

			Assert.That(settings, Is.Not.Null);
			Assert.That(settings.Endpoint, Is.Not.Empty);
			Assert.That(settings.ChatDeploymentName, Is.Not.Empty);
			Assert.That(settings.EmbeddingDeploymentName, Is.Not.Empty);
		}

		[Test]
		public void CreateClient_MissingEndpoint_Throws()
		{
			AzureOpenAISettings settings = new()
			{
				Endpoint = string.Empty,
				ApiKey = "key",
			};

			Assert.Throws<InvalidOperationException>(() => AzureOpenAIClientFactory.Create(settings));
		}

		[Test]
		public void CreateClient_MissingApiKey_Throws()
		{
			AzureOpenAISettings settings = new()
			{
				Endpoint = "https://example.openai.azure.com/",
				ApiKey = string.Empty,
			};

			Assert.Throws<InvalidOperationException>(() => AzureOpenAIClientFactory.Create(settings));
		}
	}
}
