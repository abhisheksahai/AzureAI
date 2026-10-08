using Azure.AI.OpenAI;
using Azure.Identity;
using CommunityToolkit.VectorData.Qdrant;
using Microsoft.Extensions.AI;
using Qdrant.Client;
using MAF.QuadrantVectorStore;
using Microsoft.Agents.AI;
using Microsoft.Extensions.VectorData;

var endPoint = "https://azureopenai-maf-resource.services.ai.azure.com";
var model = "gpt-5.5";

IChatClient chatClient = new AzureOpenAIClient(new Uri(endPoint), new AzureCliCredential()).GetChatClient(model).AsIChatClient();

var embeddingClient = new AzureOpenAIClient(new Uri(endPoint), new AzureCliCredential()).GetEmbeddingClient("text-embedding-3-small").AsIEmbeddingGenerator();

var quadrantClient = new QdrantClient("localhost", 6634);
var vectorStore = new QdrantVectorStore(quadrantClient, ownsClient: true);

var adrCollection = vectorStore.GetCollection<Guid, ArchitectureDecision>("enterprise_adrs");
await adrCollection.EnsureCollectionExistsAsync();

var sampleAdr = new List<ArchitectureDecision>
{
	new()
	{
		DocumentId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
		Title = "ADR-001 : Use Microservices Architecture",
		Content = "We will adopt a microservices architecture to improve scalability and maintainability.",
	},
	new()
	{
		DocumentId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
		Title = "ADR-002 : Implement CI/CD Pipeline",
		Content = "We will implement a continuous integration and continuous deployment pipeline to automate testing and deployment.",
	},
	new()
	{
		DocumentId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
		Title = "ADR-003 : Adopt Cloud-Native Technologies",
		Content = "We will adopt cloud-native technologies to leverage the benefits of cloud computing and improve our application's performance and reliability.",
	}
};


foreach (var sampleAd in sampleAdr)
{
	var embedding = await embeddingClient.GenerateAsync(sampleAd.Content);
	sampleAd.ContentVector = embedding.Vector;
	await adrCollection.UpsertAsync(sampleAd);
}

Console.WriteLine("Sample Architecture Decisions have been added to the Qdrant vector store.");

TextSearchProviderOptions textSearchOptions = new()
{
	SearchTime = TextSearchProviderOptions.TextSearchBehavior.BeforeAIInvoke
};

async Task<IEnumerable<TextSearchProvider.TextSearchResult>> VectorSearchAdaptor(string query, CancellationToken cancellationToken)
{
	var queryEmbedding = await embeddingClient.GenerateAsync(query, cancellationToken: cancellationToken);
	var queryVector = queryEmbedding.Vector;

	var searchOptions = new VectorSearchOptions<ArchitectureDecision>();
	var searchResults = adrCollection.SearchAsync(queryVector, 3, searchOptions, cancellationToken);

	// Consume with await foreach (example collection pattern)
	var results = new List<TextSearchProvider.TextSearchResult>();
	await foreach (var result in searchResults)
	{
		results.Add(new TextSearchProvider.TextSearchResult
		{
			SourceName = $"ADR : {result.Record.Title}",
			SourceLink = $"adr://{result.Record.DocumentId}",
			Text = $"Content :{result.Record.Content}",
		});
	}
	return results;
}

AIAgent architectAgent = chatClient.AsAIAgent(new ChatClientAgentOptions()
{
	Name = "EnterpriseArchitect",
	ChatOptions = new ChatOptions()
	{
		Instructions = "You are an Enterprise Architect AI agent. You will provide guidance and recommendations based on the architecture decisions stored in the vector store.",
	},
	AIContextProviders = [new TextSearchProvider(VectorSearchAdaptor, textSearchOptions)]
});

Console.WriteLine("Enterprise Architect AI Agent is ready. Type your questions below (type 'exit' to quit):");

string query = "Are we planning to adopt a microservices architecture";
Console.WriteLine(query);

AgentResponse response = await architectAgent.RunAsync(query);

Console.WriteLine($"AI Agent Response: {response.Text}");