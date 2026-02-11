using Azure;
using Azure.AI.Vision.ImageAnalysis;

namespace ComputerVision.Services;

public class AzureVisionService : IAzureVisionService
{
    private readonly ImageAnalysisClient _client;
    private readonly ILogger<AzureVisionService> _logger;

    public AzureVisionService(IConfiguration configuration, ILogger<AzureVisionService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        var endpoint = configuration["AzureVision:Endpoint"]
            ?? throw new InvalidOperationException("AzureVision:Endpoint configuration is missing");
        var apiKey = configuration["AzureVision:ApiKey"]
            ?? throw new InvalidOperationException("AzureVision:ApiKey configuration is missing");

        _client = new ImageAnalysisClient(new Uri(endpoint), new AzureKeyCredential(apiKey));
        _logger.LogInformation("Azure Vision Service initialized successfully");
    }

    public async Task<ImageAnalysisResult> AnalyzeImageAsync(Stream imageStream, CancellationToken cancellationToken = default)
    {
        try
        {
            if (imageStream == null || imageStream.Length == 0)
            {
                return new ImageAnalysisResult(
                    string.Empty,
                    0,
                    false,
                    "Image stream is empty or null"
                );
            }

            // Ensure stream is at the beginning
            if (imageStream.CanSeek)
            {
                imageStream.Position = 0;
            }

            var options = new ImageAnalysisOptions
            {
                Language = "en",
                GenderNeutralCaption = true
            };

            var result = await _client.AnalyzeAsync(
                BinaryData.FromStream(imageStream),
                VisualFeatures.Caption,
                options,
                cancellationToken
            );

            if (result?.Value?.Caption == null)
            {
                _logger.LogWarning("Azure Vision returned null caption");
                return new ImageAnalysisResult(
                    string.Empty,
                    0,
                    false,
                    "No caption was generated for the image"
                );
            }

            _logger.LogInformation("Image analyzed successfully: {Caption}", result.Value.Caption.Text);
            return new ImageAnalysisResult(
                result.Value.Caption.Text,
                result.Value.Caption.Confidence,
                true
            );
        }
        catch (RequestFailedException ex)
        {
            _logger.LogError(ex, "Azure Vision API request failed");
            return new ImageAnalysisResult(
                string.Empty,
                0,
                false,
                $"Azure Vision API error: {ex.Message}"
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error analyzing image");
            return new ImageAnalysisResult(
                string.Empty,
                0,
                false,
                "An unexpected error occurred while analyzing the image"
            );
        }
    }
}
