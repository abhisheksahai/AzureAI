using Azure.AI.OpenAI;
using OpenAI.Audio;

namespace Azure.OpenAI.Audio
{
	public class AudioTranscriptionService : IAudioTranscriptionService
	{
		private readonly AudioClient _audioClient;

		public AudioTranscriptionService(AzureOpenAIClient client, string deploymentName)
		{
			ArgumentNullException.ThrowIfNull(client);

			if (string.IsNullOrWhiteSpace(deploymentName))
			{
				throw new ArgumentException("Deployment name is required.", nameof(deploymentName));
			}

			_audioClient = client.GetAudioClient(deploymentName);
		}

		public string Transcribe(string audioFilePath)
		{
			if (string.IsNullOrWhiteSpace(audioFilePath))
			{
				throw new ArgumentException("Audio file path is required.", nameof(audioFilePath));
			}

			if (!File.Exists(audioFilePath))
			{
				throw new FileNotFoundException("Audio file was not found.", audioFilePath);
			}

			AudioTranscription transcription = _audioClient.TranscribeAudio(audioFilePath);
			return transcription.Text;
		}
	}
}
