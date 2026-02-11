namespace ComputerVision.Services;

public interface IImageValidationService
{
    ValidationResult ValidateImage(IFormFile? file);
}

public record ValidationResult(
    bool IsValid,
    string? ErrorMessage = null
);
