using System.Text.Json;
using Azure.AI.OpenAI;
using Azure.OpenAI.Configuration;
using Azure.OpenAI.FunctionCalling;
using OpenAI.Chat;

namespace Azure.OpenAI.Test
{
	public class FunctionCallingServiceTests
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
			Assert.Throws<ArgumentNullException>(
				() => new FunctionCallingService(null!, _settings.FunctionCalling.DeploymentName));
		}

		[Test]
		public void Constructor_EmptyDeploymentName_Throws()
		{
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(_settings.FunctionCalling);

			Assert.Throws<ArgumentException>(
				() => new FunctionCallingService(client, string.Empty));
		}

		[Test]
		[Explicit("Integration test - requires a valid Azure OpenAI API key in appsettings.local.json.")]
		public void CompleteWithTools_WeatherFunction_ReturnsAnswer()
		{
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(_settings.FunctionCalling);
			IFunctionCallingService service =
				new FunctionCallingService(client, _settings.FunctionCalling.DeploymentName);

			ChatTool getWeatherTool = ChatTool.CreateFunctionTool(
				functionName: "get_current_weather",
				functionDescription: "Get the current weather for a given city.",
				functionParameters: BinaryData.FromString(
					"""
					{
						"type": "object",
						"properties": {
							"city": {
								"type": "string",
								"description": "The city name, e.g. Paris"
							}
						},
						"required": ["city"]
					}
					"""));

			List<ChatMessage> messages = new()
			{
				new SystemChatMessage("You are a helpful assistant that can look up the weather."),
				new UserChatMessage("What is the weather like in Paris right now?"),
			};

			bool toolWasCalled = false;

			string answer = service.CompleteWithTools(
				messages,
				new[] { getWeatherTool },
				toolInvoker: (functionName, argumentsJson) =>
				{
					toolWasCalled = true;
					TestContext.Out.WriteLine($"Tool requested: {functionName}({argumentsJson})");

					Assert.That(functionName, Is.EqualTo("get_current_weather"));

					using JsonDocument args = JsonDocument.Parse(argumentsJson);
					string city = args.RootElement.GetProperty("city").GetString() ?? "unknown";

					// Simulate calling a real weather API.
					return JsonSerializer.Serialize(new { city, temperatureC = 18, condition = "Sunny" });
				});

			TestContext.Out.WriteLine($"Final answer: {answer}");

			Assert.Multiple(() =>
			{
				Assert.That(toolWasCalled, Is.True, "The model was expected to invoke the weather tool.");
				Assert.That(answer, Is.Not.Empty);
			});
		}

		[Test]
		[Explicit("Integration test - requires a valid Azure OpenAI API key in appsettings.local.json.")]
		public void CompleteWithTools_StockPriceFunction_ReturnsAnswer()
		{
			AzureOpenAIClient client = AzureOpenAIClientFactory.Create(_settings.FunctionCalling);
			IFunctionCallingService service =
				new FunctionCallingService(client, _settings.FunctionCalling.DeploymentName);

			ChatTool getStockPriceTool = ChatTool.CreateFunctionTool(
				functionName: "get_stock_price",
				functionDescription: "Get the current stock price for the given symbol.",
				functionParameters: BinaryData.FromString(
					"""
					{
						"type": "object",
						"properties": {
							"symbol": {
								"type": "string",
								"description": "The ticker symbol, e.g. AAPL"
							}
						},
						"required": ["symbol"]
					}
					"""));

			List<ChatMessage> messages = new()
			{
				new SystemChatMessage("You are a financial assistant. You know company ticker symbols, "
					+ "but you must use the get_stock_price tool to look up any price."),
				new UserChatMessage("What is the ticker symbol and current stock price for Apple?"),
			};

			bool toolWasCalled = false;
			string? requestedSymbol = null;

			string answer = service.CompleteWithTools(
				messages,
				new[] { getStockPriceTool },
				toolInvoker: (functionName, argumentsJson) =>
				{
					toolWasCalled = true;
					TestContext.Out.WriteLine($"Tool requested: {functionName}({argumentsJson})");

					Assert.That(functionName, Is.EqualTo("get_stock_price"));

					using JsonDocument args = JsonDocument.Parse(argumentsJson);
					requestedSymbol = args.RootElement.GetProperty("symbol").GetString();

					// The model supplies the symbol; the dummy function returns the price.
					return GetStockPrice(requestedSymbol!);
				});

			TestContext.Out.WriteLine($"Final answer: {answer}");

			string normalizedAnswer = answer.Replace(",", string.Empty);

			Assert.Multiple(() =>
			{
				Assert.That(toolWasCalled, Is.True, "The model was expected to invoke the stock-price tool.");
				Assert.That(requestedSymbol, Is.Not.Null.And.Not.Empty, "The model should supply a ticker symbol.");
				Assert.That(normalizedAnswer, Does.Contain("1000"));
			});
		}

		/// <summary>
		/// Dummy stock-price function (mirrors the reference get_stock_price).
		/// The symbol is supplied by the model; this returns a fixed dummy price.
		/// </summary>
		private static string GetStockPrice(string symbol)
		{
			return JsonSerializer.Serialize(new { symbol, price = "1000" });
		}
	}
}
