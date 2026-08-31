using Azure.AI.OpenAI;
using OpenAI.Images;

namespace Azure.OpenAI.Image
{
	public class ImageGenerationService : IImageGenerationService
	{
		private readonly ImageClient _imageClient;

		public ImageGenerationService(AzureOpenAIClient client, string deploymentName)
		{
			ArgumentNullException.ThrowIfNull(client);

			if (string.IsNullOrWhiteSpace(deploymentName))
			{
				throw new ArgumentException("Deployment name is required.", nameof(deploymentName));
			}

			_imageClient = client.GetImageClient(deploymentName);
		}

		public byte[] GenerateImage(string prompt)
		{
			if (string.IsNullOrWhiteSpace(prompt))
			{
				throw new ArgumentException("Prompt is required.", nameof(prompt));
			}

			ImageGenerationOptions options = new()
			{
				Size = GeneratedImageSize.W1024xH1024,
				ResponseFormat = GeneratedImageFormat.Bytes
			};

			GeneratedImage image = _imageClient.GenerateImage(prompt, options);
			return image.ImageBytes.ToArray();
		}
	}
}
