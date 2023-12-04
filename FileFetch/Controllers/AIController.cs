using FileFetch.Helpers;
using FileFetch.Models;
using FileFetch.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FileFetch.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIController : ControllerBase
    {
        private readonly IChatService _chatService;

        public AIController(IChatService chatService)
        {
            _chatService = chatService;
        }

        [HttpPost("chat")]
        public async Task<IActionResult> Chat([FromForm] ChatModel chatModel)
        {
            try
            {
                if (!FileHelper.IsValidFile(chatModel.File))
                {
                    return BadRequest("File not Supported");
                }
                var chatResponse = await _chatService.GetChatResponse(chatModel);
                return Ok(chatResponse);
            }
            catch (Exception ex)
            {

                return StatusCode(500, "Chat Server error");

            }
        }
    }
}
