namespace ComputerVision.Services;

public class ImageValidationService : IImageValidationService
{
    private readonly long _maxFileSizeBytes;
    private readonly HashSet<string> _allowedContentTypes;
    private readonly ILogger<ImageValidationService> _logger;

    public ImageValidationService(IConfiguration configuration, ILogger<ImageValidationService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _maxFileSizeBytes = configuration.GetValue<long>("ImageUpload:MaxFileSizeBytes", 10 * 1024 * 1024); // Default 10MB

        var allowedTypes = configuration.GetSection("ImageUpload:AllowedContentTypes").Get<string[]>()
            ?? new[] { "image/jpeg", "image/png", "image/gif", "image/bmp" };

        _allowedContentTypes = new HashSet<string>(allowedTypes, StringComparer.OrdinalIgnoreCase);
    }

    public ValidationResult ValidateImage(IFormFile? file)
    {
        if (file == null)
        {
            return new ValidationResult(false, "No image file was uploaded");
        }

        if (file.Length == 0)
        {
            return new ValidationResult(false, "The uploaded file is empty");
        }

        if (file.Length > _maxFileSizeBytes)
        {
            var maxSizeMB = _maxFileSizeBytes / (1024.0 * 1024.0);
            return new ValidationResult(false, $"File size exceeds maximum allowed size of {maxSizeMB:F1} MB");
        }

        if (!_allowedContentTypes.Contains(file.ContentType))
        {
            return new ValidationResult(false, $"File type '{file.ContentType}' is not allowed. Allowed types: {string.Join(", ", _allowedContentTypes)}");
        }

        _logger.LogDebug("Image validation passed for file: {FileName}", file.FileName);
        return new ValidationResult(true);
    }
}
