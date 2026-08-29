using Azure.AI.OpenAI;
using Azure.OpenAI.Audio;
using Azure.OpenAI.Configuration;

namespace Azure.OpenAI.Test
{
	public class AudioTranscriptionServiceTests
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
				() => new AudioTranscriptionService(null!, _settings.Whisper.DeploymentName));
		}

		[Test]
		public void Constructor_EmptyDeploymentName_Throws()
		{
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(_settings.Whisper);

			Assert.Throws<ArgumentException>(
				() => new AudioTranscriptionService(client, string.Empty));
		}

		[Test]
		public void Transcribe_EmptyPath_Throws()
		{
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(_settings.Whisper);
			IAudioTranscriptionService service =
				new AudioTranscriptionService(client, _settings.Whisper.DeploymentName);

			Assert.Throws<ArgumentException>(() => service.Transcribe(string.Empty));
		}

		[Test]
		public void Transcribe_MissingFile_Throws()
		{
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(_settings.Whisper);
			IAudioTranscriptionService service =
				new AudioTranscriptionService(client, _settings.Whisper.DeploymentName);

			Assert.Throws<FileNotFoundException>(
				() => service.Transcribe("does-not-exist.mp3"));
		}

		[Test]
		[Explicit("Integration test - requires a valid Whisper API key in appsettings.local.json and an audio file.")]
		public void Transcribe_ReturnsTranscript()
		{
			string audioFilePath = Path.Combine(TestContext.CurrentContext.TestDirectory, "Audio", "aws_lambda.mp3");
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(_settings.Whisper);
			IAudioTranscriptionService service = new AudioTranscriptionService(client, _settings.Whisper.DeploymentName);

			string transcript = service.Transcribe(audioFilePath);

			TestContext.Out.WriteLine($"Transcript: {transcript}");

			Assert.That(transcript, Is.Not.Null.And.Not.Empty);
		}
	}
}
