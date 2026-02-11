namespace ComputerVision.Services;

public interface IAzureVisionService
{
    Task<ImageAnalysisResult> AnalyzeImageAsync(Stream imageStream, CancellationToken cancellationToken = default);
}

public record ImageAnalysisResult(
    string Caption,
    double Confidence,
    bool Success,
    string? ErrorMessage = null
);
