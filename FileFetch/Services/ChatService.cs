using Azure;
using Azure.AI.OpenAI;
using Azure.Core.Pipeline;
using FileFetch.Helpers;
using FileFetch.Models;
using FileFetch.Prompts;
using SharpToken;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;

namespace FileFetch.Services
{
    public class ChatService : IChatService
    {
        private readonly IOpenAIService _openAIService;
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;

        public ChatService(IOpenAIService openAIService, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _openAIService = openAIService;
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }
        public async Task<string> GetChatResponse(ChatModel chatModel)
        {
            string response = string.Empty;
            try
            {
                List<string> documentContent = new List<string>();
                var fileData = FileHelper.GetFileData(chatModel.File);
                MemoryStream stream = new MemoryStream(fileData);
                var docParagraphs = await _openAIService.GetDocumentParagraphs(stream);
                foreach (var paragraph in docParagraphs)
                {
                    documentContent.Add(paragraph.Content);
                }

                response = await GetDataFromContent(chatModel.ChatMessage, documentContent);

            }
            catch (Exception ex)
            {
                throw;
            }
            return response;
        }

        #region private AI methods
        private int CountTokens(string text)
        {
            var encoding = GptEncoding.GetEncodingForModel(_configuration["OpenAI:BaseModel"]);
            var response = encoding.Encode(text);
            return response.Count;
        }
        private async IAsyncEnumerable<string> MakeChatCompletion(ChatCompletionsOptions chatCompletionsOptions, [EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var _aIClient = new OpenAIClient(
                   new Uri(_configuration["OpenAI:URL"]),
            new AzureKeyCredential(_configuration["OpenAI:Key"]),
                   new OpenAIClientOptions
                   {
                       Transport = new HttpClientTransport(_httpClientFactory.CreateClient())
                   }
                );
            var aiResponse = await _aIClient.GetChatCompletionsStreamingAsync(_configuration["OpenAI:Model"], chatCompletionsOptions, cancellationToken);
            using (var streamingCompletion = aiResponse.Value)
            {
                await foreach (var choice in streamingCompletion.GetChoicesStreaming(cancellationToken))
                {
                    var messageBuilder = new StringBuilder();



                    await foreach (var message in choice.GetMessageStreaming(cancellationToken))
                    {
                        if (message.Content != null)
                        {
                            messageBuilder.Append(message.Content);
                        }
                    }



                    yield return messageBuilder.ToString();
                }
            }
        }
        private async Task<string> GetDataFromContent(string userInput, List<string> fileContent)
        {
            string outputContent = string.Empty;
            try
            {
                var completionOptions = new ChatCompletionsOptions()
                {
                    Temperature = 0.0f,
                    MaxTokens = Convert.ToInt32(_configuration["OpenAI:MaxToken"]),
                    NucleusSamplingFactor = 0.9f,
                    FrequencyPenalty = 0,
                    PresencePenalty = 0
                };
                var prompt = ChatPrompts.GetChatMessagePrompt(string.Join(" ", fileContent));
                int tokenCount = CountTokens(prompt);

                if (tokenCount > completionOptions.MaxTokens)
                {
                    var largePrompt = ChatPrompts.GetChatMessagePromptForLargeFile("", "");
                    string chunkText = string.Empty;
                    string initialTextWithoutContent = largePrompt;
                    int initialtokenCount = CountTokens(initialTextWithoutContent);
                    int tokenCountWithSentence = 0;
                    int sentenceCounter = 0;
                    int leftSentenceIndex = 0;
                    foreach (var paragraph in fileContent)
                    {
                        if (leftSentenceIndex != 0)
                        {
                            tokenCountWithSentence = tokenCountWithSentence == 0
                                ? initialtokenCount + CountTokens(fileContent[leftSentenceIndex])
                                : tokenCountWithSentence + CountTokens(fileContent[leftSentenceIndex]);
                            chunkText += fileContent[leftSentenceIndex];
                            leftSentenceIndex = 0;
                        }
                        tokenCountWithSentence = tokenCountWithSentence == 0
                            ? initialtokenCount + CountTokens(paragraph)
                            : tokenCountWithSentence + CountTokens(paragraph);
                        if (tokenCountWithSentence > completionOptions.MaxTokens || sentenceCounter == fileContent.Count - 1)
                        {
                            leftSentenceIndex = sentenceCounter;
                            var largeTextCompletionOptions = new ChatCompletionsOptions()
                            {
                                Temperature = 0.0f,
                                MaxTokens = 2000,
                                NucleusSamplingFactor = 0.95f,
                                FrequencyPenalty = 0,
                                PresencePenalty = 0
                            };
                            var largeContentPrompt = ChatPrompts.GetChatMessagePromptForLargeFile(outputContent, chunkText);

                            largeTextCompletionOptions.Messages.Add(new ChatMessage(ChatRole.System, largeContentPrompt));
                            largeTextCompletionOptions.Messages.Add(new ChatMessage(ChatRole.User, userInput));
                            string newOutput = string.Empty;
                            await foreach (var output in MakeChatCompletion(largeTextCompletionOptions, new CancellationToken()))
                            {
                                newOutput += output;
                            }
                            outputContent = newOutput;
                            chunkText = string.Empty;
                            var newPromptForInitial = ChatPrompts.GetChatMessagePromptForLargeFile(outputContent, chunkText);
                            initialTextWithoutContent = newPromptForInitial;

                            initialtokenCount = CountTokens(initialTextWithoutContent);
                            tokenCountWithSentence = 0;
                        }
                        else
                        {
                            chunkText += paragraph.Replace("\"", "").Replace("'", "").Replace("‘", "");
                        }
                        sentenceCounter++;
                    }

                }
                else
                {
                    completionOptions.Messages.Add(new ChatMessage(ChatRole.System, prompt));
                    completionOptions.Messages.Add(new ChatMessage(ChatRole.User, userInput));
                    await foreach (var outPut in MakeChatCompletion(completionOptions, new CancellationToken()))
                    {
                        outputContent = outPut;
                    }
                }

            }
            catch (Exception)
            {
                throw;
            }
            return outputContent;
        }
        #endregion

    }
}
