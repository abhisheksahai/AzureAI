using Azure;
using Azure.AI.TextAnalytics;

string key = "2rSyj5fuGEEoUVnbnbQYXg6D9cIxpP4JlzO9ePSvFYMLUR5oOCJ8JQQJ99BLACYeBjFXJ3w3AAAaACOGeWJz";
string endpoint = "https://azure-ai-language-udemy.cognitiveservices.azure.com/";
AzureKeyCredential azureKeyCredential = new(key);
Uri languageEndPoint = new(endpoint);

var client = new TextAnalyticsClient(languageEndPoint, azureKeyCredential);

var sentences = new List<string>
{
	"I love programming in C#.",
	"I love the new Indian dhaba",
	"The food is not good at all.",
};

AnalyzeSentimentResultCollection sentencesToAnalyse = client.AnalyzeSentimentBatch(sentences);
foreach (AnalyzeSentimentResult sentence in sentencesToAnalyse)
{
	Console.WriteLine($"  Sentense Sentiment: {sentence.DocumentSentiment.Sentiment}");
	Console.WriteLine($"  Positive Score: {sentence.DocumentSentiment.ConfidenceScores.Positive}");
	Console.WriteLine($"  Neutral Score:  {sentence.DocumentSentiment.ConfidenceScores.Neutral}");
	Console.WriteLine($"  Negative Score:  {sentence.DocumentSentiment.ConfidenceScores.Negative}");

	foreach (var sentenceSentiment in sentence.DocumentSentiment.Sentences)
	{
		Console.WriteLine($"  Sentense Aspect: {sentenceSentiment.Text}");
		Console.WriteLine($"  Sentiment: {sentenceSentiment.Sentiment}");
		Console.WriteLine($"  Positive Score: {sentenceSentiment.ConfidenceScores.Positive}");
		Console.WriteLine($"  Neutral Score:  {sentenceSentiment.ConfidenceScores.Neutral}");
		Console.WriteLine($"  Negative Score:  {sentenceSentiment.ConfidenceScores.Negative}");

		foreach (var sentenceOpinion in sentenceSentiment.Opinions)
		{
			Console.WriteLine($"  Sentense Opinion: {sentenceOpinion.Target.Text}");
			Console.WriteLine($"  Sentiment: {sentenceOpinion.Target.Sentiment}");
			Console.WriteLine($"  Positive Score: {sentenceOpinion.Target.ConfidenceScores.Positive}");
			Console.WriteLine($"  Neutral Score:  {sentenceOpinion.Target.ConfidenceScores.Neutral}");
			Console.WriteLine($"  Negative Score:  {sentenceOpinion.Target.ConfidenceScores.Negative}");

			foreach (var assessment in sentenceOpinion.Assessments)
			{
				Console.WriteLine($"  Related assestment: {assessment.Text}, Value : {assessment.Sentiment}");
				Console.WriteLine($"  Related assestment positive score: {assessment.ConfidenceScores.Positive:0.00}");
				Console.WriteLine($"  Related assestment negative score: {assessment.ConfidenceScores.Negative:0.00}");
			}
		}
	}
	Console.WriteLine();
}
Console.ReadLine();