using Azure;
using Azure.AI.FormRecognizer.DocumentAnalysis;
using Azure.Core.Pipeline;
using System.Net.Http;
using System.Threading;

namespace FileFetch.Services
{
    public class OpenAIService : IOpenAIService
    {
        private string _apiKey;
        private string _serviceURL;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public OpenAIService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<List<DocumentParagraph>> GetDocumentParagraphs(Stream fileStream)
        {
            DocumentAnalysisClient documentAnalysisClient = CreateClient();
            AnalyzeDocumentOperation operation = await documentAnalysisClient.AnalyzeDocumentAsync(WaitUntil.Completed, "prebuilt-read", fileStream);
            AnalyzeResult result = operation.Value;
            return result.Paragraphs.ToList();
        }

        private DocumentAnalysisClient CreateClient()
        {
            _apiKey = _configuration["FormRecognizer:Key"];
            _serviceURL = _configuration["FormRecognizer:URL"];
            var keyCredintials = new AzureKeyCredential(_apiKey);
            var client = new DocumentAnalysisClient(
                   new Uri(_serviceURL),
                  keyCredintials,
                   new DocumentAnalysisClientOptions()
                   {
                       Transport = new HttpClientTransport(_httpClientFactory.CreateClient())
                   }
               );
            return client;
        }
    }
}
