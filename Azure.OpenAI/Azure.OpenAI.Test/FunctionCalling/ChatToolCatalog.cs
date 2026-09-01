using OpenAI.Chat;

namespace Azure.OpenAI.Test.FunctionCalling
{
	/// <summary>
	/// Central catalog of reusable <see cref="ChatTool"/> function definitions.
	/// Each function is a separate <see cref="ChatTool"/> (the SDK allows only one
	/// function per tool), but <see cref="All"/> bundles them so callers can pass
	/// every available function to the model in a single collection.
	/// </summary>
	public static class ChatToolCatalog
	{
		public static ChatTool GetCurrentWeather { get; } = ChatTool.CreateFunctionTool(
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

		public static ChatTool GetStockPrice { get; } = ChatTool.CreateFunctionTool(
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

		/// <summary>
		/// All available function tools in one collection. Pass this to the model
		/// when you want it to be able to choose from every function at once.
		/// </summary>
		public static IReadOnlyList<ChatTool> All { get; } = new[]
		{
			GetCurrentWeather,
			GetStockPrice,
		};
	}
}
