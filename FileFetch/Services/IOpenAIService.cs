using Azure.AI.FormRecognizer.DocumentAnalysis;

namespace FileFetch.Services
{
    public interface IOpenAIService
    {
        Task<List<DocumentParagraph>> GetDocumentParagraphs(Stream fileStream);
    }
}
