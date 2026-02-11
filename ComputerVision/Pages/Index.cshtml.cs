using ComputerVision.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ComputerVision.Pages;

public class IndexModel : PageModel
{
    private readonly IAzureVisionService _visionService;
    private readonly IImageValidationService _validationService;
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(
        IAzureVisionService visionService,
        IImageValidationService validationService,
        ILogger<IndexModel> logger)
    {
        _visionService = visionService ?? throw new ArgumentNullException(nameof(visionService));
        _validationService = validationService ?? throw new ArgumentNullException(nameof(validationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public class ImageCaptionModel
    {
        public IFormFile? Image { get; set; }
        public string? ImageData { get; set; }
        public string? ImageCaption { get; set; }
        public double? Confidence { get; set; }
        public string? ErrorMessage { get; set; }
    }

    [BindProperty]
    public ImageCaptionModel Input { get; set; } = new();

    public void OnGet()
    {
        // Initialize with empty model
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Validate the uploaded image
            var validationResult = _validationService.ValidateImage(Input.Image);
            if (!validationResult.IsValid)
            {
                Input.ErrorMessage = validationResult.ErrorMessage;
                _logger.LogWarning("Image validation failed: {Error}", validationResult.ErrorMessage);
                return Page();
            }

            // Process the image
            using var memoryStream = new MemoryStream();
            await Input.Image!.CopyToAsync(memoryStream, cancellationToken);

            // Convert to Base64 for display
            memoryStream.Position = 0;
            Input.ImageData = Convert.ToBase64String(memoryStream.ToArray());

            // Reset stream position for analysis
            memoryStream.Position = 0;

            // Analyze the image
            var analysisResult = await _visionService.AnalyzeImageAsync(memoryStream, cancellationToken);

            if (analysisResult.Success)
            {
                Input.ImageCaption = analysisResult.Caption;
                Input.Confidence = analysisResult.Confidence;
                _logger.LogInformation("Image processed successfully for user");
            }
            else
            {
                Input.ErrorMessage = analysisResult.ErrorMessage;
                _logger.LogWarning("Image analysis failed: {Error}", analysisResult.ErrorMessage);
            }
        }
        catch (OperationCanceledException)
        {
            Input.ErrorMessage = "The operation was cancelled";
            _logger.LogInformation("Image analysis operation was cancelled");
        }
        catch (Exception ex)
        {
            Input.ErrorMessage = "An unexpected error occurred while processing your image";
            _logger.LogError(ex, "Unexpected error in OnPostAsync");
        }

        return Page();
    }
}
