namespace Azure.OpenAI.Audio
{
	public interface IAudioTranscriptionService
	{
		/// <summary>
		/// Transcribes the audio file at the given path into text using the Whisper model.
		/// </summary>
		/// <param name="audioFilePath">Path to a supported audio file (mp3, wav, m4a, ...).</param>
		/// <returns>The transcribed text.</returns>
		string Transcribe(string audioFilePath);
	}
}
