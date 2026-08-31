using Azure.AI.OpenAI;
using Azure.OpenAI.Configuration;
using Azure.OpenAI.Image;

namespace Azure.OpenAI.Test
{
	public class ImageGenerationServiceTests
	{
		private AzureOpenAISettings _settings = null!;

		[SetUp]
		public void Setup()
		{
			_settings = AzureOpenAIConfiguration.Load();
		}

		[Test]
		public void Constructor_NullClient_Throws()
		{
			Assert.Throws<ArgumentNullException>(
				() => new ImageGenerationService(null!, _settings.ImageGeneration.DeploymentName));
		}

		[Test]
		public void Constructor_EmptyDeploymentName_Throws()
		{
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(_settings.ImageGeneration);

			Assert.Throws<ArgumentException>(
				() => new ImageGenerationService(client, string.Empty));
		}

		[Test]
		public void GenerateImage_EmptyPrompt_Throws()
		{
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(_settings.ImageGeneration);
			IImageGenerationService service =
				new ImageGenerationService(client, _settings.ImageGeneration.DeploymentName);

			Assert.Throws<ArgumentException>(() => service.GenerateImage(string.Empty));
		}

		[Test]
		[Explicit("Integration test - requires a valid Image Generation API key in appsettings.local.json.")]
		public void GenerateImage_ReturnsImageBytes()
		{
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(_settings.ImageGeneration);
			IImageGenerationService service =
				new ImageGenerationService(client, _settings.ImageGeneration.DeploymentName);

			byte[] imageBytes = service.GenerateImage("A programmer on the moon");

			Assert.That(imageBytes, Is.Not.Null);
			Assert.That(imageBytes.Length, Is.GreaterThan(0));

			string outputPath = Path.Combine(TestContext.CurrentContext.TestDirectory, "generated-programmer-on-the-moon.png");
			File.WriteAllBytes(outputPath, imageBytes);
			TestContext.Out.WriteLine($"Generated image ({imageBytes.Length} bytes) saved to: {outputPath}");
		}
	}
}
