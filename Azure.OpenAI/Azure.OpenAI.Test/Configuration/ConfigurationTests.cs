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
			Assert.That(settings.ChatCompletion.Endpoint, Is.Not.Empty);
			Assert.That(settings.ChatCompletion.DeploymentName, Is.Not.Empty);
			Assert.That(settings.Embedding.Endpoint, Is.Not.Empty);
			Assert.That(settings.Embedding.DeploymentName, Is.Not.Empty);
		}

		[Test]
		public void CreateClient_MissingEndpoint_Throws()
		{
			AzureOpenAIResourceSettings settings = new()
			{
				Endpoint = string.Empty,
				ApiKey = "key",
			};

			Assert.Throws<InvalidOperationException>(() => AzureOpenAIClientFactory.Create(settings));
		}

		[Test]
		public void CreateClient_MissingApiKey_Throws()
		{
			AzureOpenAIResourceSettings settings = new()
			{
				Endpoint = "https://example.openai.azure.com/",
				ApiKey = string.Empty,
			};

			Assert.Throws<InvalidOperationException>(() => AzureOpenAIClientFactory.Create(settings));
		}
	}
}
