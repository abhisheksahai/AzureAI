namespace Azure.OpenAI.Image
{
	public interface IImageGenerationService
	{
		/// <summary>
		/// Generates an image from a text prompt and returns the raw image bytes.
		/// </summary>
		/// <param name="prompt">A text description of the desired image.</param>
		/// <returns>The generated image encoded as PNG bytes.</returns>
		byte[] GenerateImage(string prompt);
	}
}
