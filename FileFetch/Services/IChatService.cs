using FileFetch.Models;

namespace FileFetch.Services
{
    public interface IChatService
    {
        Task<string> GetChatResponse(ChatModel chatModel);

    }
}
