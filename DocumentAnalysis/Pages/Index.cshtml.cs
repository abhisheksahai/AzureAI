using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DocumentAnalysis.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public IndexModel(ILogger<IndexModel> logger)
    {
        _logger = logger;
    }

    [BindProperty]
    public IFormFile Document { get; set; }

    public void OnGet()
    {

    }

    public async Task<IActionResult> OnPostAsync()
    {
        stringe endpoint="https://azure-ai-documentintelligence.cognitiveservices.azure.com/";
        string key="EuirCu05iY4oCfNb0Jq6vpJQEWJJhR8OPnFiatg6fXMTsBQlLGSqJQQJ99CBACYeBjFXJ3w3AAALACOG01ed";
        return RedirectToPage("Success");
    }
}