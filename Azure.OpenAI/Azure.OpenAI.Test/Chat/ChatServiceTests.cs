using Azure.AI.OpenAI;
using Azure.OpenAI.Chat;
using Azure.OpenAI.Configuration;
using OpenAI.Chat;

namespace Azure.OpenAI.Test
{
	public class ChatServiceTests
	{
		private AzureOpenAISettings _settings = null!;

		[SetUp]
		public void Setup()
		{
			_settings = AzureOpenAIConfiguration.Load();
		}

		[Test]
		public void Constructor_NullClient_Throws()
		{
			Assert.Throws<ArgumentNullException>(() => new ChatService(null!, _settings.ChatCompletion.DeploymentName));
		}

		[Test]
		public void Constructor_EmptyDeploymentName_Throws()
		{
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(_settings.ChatCompletion);

			Assert.Throws<ArgumentException>(() => new ChatService(client, string.Empty));
		}

		[Test]
		public void DefaultOptions_HasExpectedValues()
		{
			ChatCompletionOptions options = ChatService.DefaultOptions;

			Assert.That(options.Temperature, Is.EqualTo(0.7f));
			Assert.That(options.MaxOutputTokenCount, Is.EqualTo(1000));
		}

		[Test]
		[Explicit("Integration test - requires a valid Azure OpenAI API key in appsettings.local.json.")]
		public void CompleteChat_ReturnsResponse()
		{
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(_settings.ChatCompletion);
			IChatService chatService = new ChatService(client, _settings.ChatCompletion.DeploymentName);

			List<ChatMessage> messages = new()
			{
				new SystemChatMessage("You are a helpful assistant."),
				new UserChatMessage("Say hello in one word."),
			};

			string response = chatService.CompleteChat(messages);

			Assert.That(response, Is.Not.Empty);
		}

		[Test]
		[Explicit("Integration test - requires a valid Azure OpenAI API key in appsettings.local.json.")]
		public void CompleteChat_ParisTravelTips_ReturnsResponse()
		{
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(_settings.ChatCompletion);
			IChatService chatService = new ChatService(client, _settings.ChatCompletion.DeploymentName);

			List<ChatMessage> messages = new()
			{
				new SystemChatMessage("You are a travel planner."),
				new UserChatMessage("I am going to Paris, what should I see?"),
			};

			ChatCompletionOptions options = new()
			{
				MaxOutputTokenCount = 13107,
				Temperature = 1.0f,
				TopP = 1.0f,
				FrequencyPenalty = 0.0f,
				PresencePenalty = 0.0f,
			};

			string response = chatService.CompleteChat(messages, options);

			TestContext.Out.WriteLine(response);

			Assert.That(response, Is.Not.Empty);
		}

		[Test]
		[Explicit("Integration test - requires a valid Azure OpenAI API key in appsettings.local.json.")]
		public void CompleteChat_AstrologyBirthChart_ReturnsPrediction()
		{
			// Birth details supplied by the user.
			const string name = "Abhishek Sahai";
			const string dateOfBirth = "05 May 1986";
			const string timeOfBirth = "01:35 PM";
			const string placeOfBirth = "Arrah, Bihar, India";

			// A language model cannot reliably compute the Moon sign (Rashi) or the Ascendant
			// (Lagna) from raw birth data because that requires precise astronomical/ephemeris
			// calculations. To avoid wrong results (e.g. Makar instead of Meen, or Kanya instead
			// of Singh), the user's known Rashi and Lagna are supplied here and treated as
			// authoritative by the model.
			const string moonSignRashi = "Meen (Pisces)";
			const string ascendantLagna = "Singh (Leo)";

			// Prediction window is derived dynamically: current month -> end of current year,
			// followed by a summary of the next year.
			DateTime today = DateTime.Now;
			int currentYear = today.Year;
			int nextYear = currentYear + 1;
			string currentMonthName = today.ToString("MMMM", System.Globalization.CultureInfo.InvariantCulture);

			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(_settings.ChatCompletion);
			IChatService chatService = new ChatService(client, _settings.ChatCompletion.DeploymentName);

			List<ChatMessage> messages = new()
			{
				new SystemChatMessage(
					"You are an expert Vedic astrologer. "
					+ "You follow the Moon sign (Rashi) system, NOT the Sun sign. "
					+ "The user's Moon sign (Rashi) and Ascendant (Lagna) are provided to you as "
					+ "AUTHORITATIVE facts. You MUST use the given Rashi and Lagna exactly as provided "
					+ "and MUST NOT recalculate or change them. Base ALL predictions on the provided Moon sign. "
					+ "Given a person's name, date of birth, time of birth and place of birth, "
					+ "generate a detailed natal birth chart followed by predictions. "
					+ "IMPORTANT: Even though these instructions are in English, you MUST respond "
					+ "ENTIRELY in Hindi (हिंदी) using Devanagari script. "
					+ "Respond in clean, well-structured Markdown using the following sections "
					+ "(translate the headings into Hindi):\n"
					+ "## Birth Details\n"
					+ "## Birth Chart\n"
					+ "- Ascendant (Lagna) — use only the provided value\n"
					+ "- Moon Sign (Rashi) — use only the provided value\n"
					+ "- Planetary Positions (a Markdown table with columns: Planet | Sign | House)\n"
					+ "## Prediction for This Year (from the current month to year end)\n"
					+ "In this section give a month-by-month prediction from the current month to the "
					+ "end of the year. For each month, cover career, finance, health and relationships.\n"
					+ "## Summary for Next Year\n"
					+ "Give a short summary for the next year (career, finance, health, relationships and "
					+ "overall outlook). Keep the language positive, clear and easy to understand for a layperson."),
				new UserChatMessage(
					$"Name: {name}\n"
					+ $"Date of Birth: {dateOfBirth}\n"
					+ $"Time of Birth: {timeOfBirth}\n"
					+ $"Place of Birth: {placeOfBirth}\n"
					+ $"Ascendant (Lagna — authoritative, do NOT change): {ascendantLagna}\n"
					+ $"Moon Sign (Rashi — authoritative, do NOT change): {moonSignRashi}\n\n"
					+ $"Please prepare my birth chart based on the given Moon sign ({moonSignRashi}) "
					+ $"and Ascendant ({ascendantLagna}). "
					+ "Keep the Rashi and Lagna exactly as provided above; do not recalculate them. "
					+ $"Then give a detailed month-by-month astrology prediction from {currentMonthName} {currentYear} "
					+ $"to the end of {currentYear}, followed by a short summary for the next year {nextYear}. "
					+ "Provide the entire response in Hindi in a good, readable format."),
			};

			ChatCompletionOptions options = new()
			{
				MaxOutputTokenCount = 13107,
				Temperature = 0.8f,
				TopP = 0.95f,
				FrequencyPenalty = 0.0f,
				PresencePenalty = 0.0f,
			};

			string response = chatService.CompleteChat(messages, options);

			TestContext.Out.WriteLine(response);

			Assert.Multiple(() =>
			{
				Assert.That(response, Is.Not.Empty, "The astrologer should return a birth chart and prediction.");
				Assert.That(response, Does.Contain("कुंडली"), "The response should include the birth chart (कुंडली) section in Hindi.");
				Assert.That(response, Does.Contain("राशि"), "The response should reference the Moon sign (राशि).");
				Assert.That(response, Does.Contain("मीन"), "The response must use the authoritative Moon sign (मीन / Meen).");
				Assert.That(response, Does.Contain(currentYear.ToString()), "The response should reference the current year.");
				Assert.That(response, Does.Contain(nextYear.ToString()), "The response should include a summary for the next year.");
			});
		}

	}
}