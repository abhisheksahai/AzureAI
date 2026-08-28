using Azure.OpenAI.Configuration;

namespace Azure.OpenAI.Test
{
	public class ConfigurationTests
	{
		private AzureOpenAISettings _settings = null!;

		[SetUp]
		public void Setup()
		{
			_settings = AzureOpenAIConfiguration.Load();
		}

		[Test]
		public void Load_BindsAzureOpenAISection()
		{
			Assert.That(_settings, Is.Not.Null);
			Assert.That(_settings.ChatCompletion.Endpoint, Is.Not.Empty);
			Assert.That(_settings.ChatCompletion.DeploymentName, Is.Not.Empty);
			Assert.That(_settings.Embedding.Endpoint, Is.Not.Empty);
			Assert.That(_settings.Embedding.DeploymentName, Is.Not.Empty);
		}

		[Test]
		public void CreateClient_MissingEndpoint_Throws()
		{
			AzureOpenAIResourceSettings settings = new()
			{
				Endpoint = string.Empty,
				ApiKey = _settings.ChatCompletion.ApiKey,
				DeploymentName = _settings.ChatCompletion.DeploymentName,
			};

			Assert.Throws<InvalidOperationException>(() => AzureOpenAIClientFactory.Create(settings));
		}

		[Test]
		public void CreateClient_MissingApiKey_Throws()
		{
			AzureOpenAIResourceSettings settings = new()
			{
				Endpoint = _settings.ChatCompletion.Endpoint,
				ApiKey = string.Empty,
				DeploymentName = _settings.ChatCompletion.DeploymentName,
			};

			Assert.Throws<InvalidOperationException>(() => AzureOpenAIClientFactory.Create(settings));
		}
	}
}
