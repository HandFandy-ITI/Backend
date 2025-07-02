using System.Text.Json;
using System.Text;
using OstaFandy.PL.BL.IBL;
using Microsoft.Extensions.Caching.Memory;
using OstaFandy.DAL.Repos.IRepos;

namespace OstaFandy.PL.BL
{
    public class ChatBotService : IChatBotService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly IMemoryCache _cache;
        private readonly IUnitOfWork _unitOfWork;

        public ChatBotService(HttpClient httpClient, IConfiguration config,IMemoryCache cache,IUnitOfWork unitOfWork)
        {
            _httpClient = httpClient;
            _apiKey = config["OpenRouter:ApiKey"];
            _unitOfWork= unitOfWork;
            _cache = cache;
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<string> ChatbotHandeller(string userId, String userMessage)
        {

            if (userId == null || userMessage == null)
            {
                return "";
            }
            string cachkey = $"chat_history_{userId}";

            if (!_cache.TryGetValue(cachkey, out List<string> chatHistory))
            {
                chatHistory = new List<string>();
            }

            chatHistory.Add($"User:{userMessage}");

            if (chatHistory.Count > 10)
            {
                chatHistory = chatHistory.Skip(chatHistory.Count - 10).ToList();

            }

            var services = _unitOfWork.ServiceRepo.GetAll(s => s.IsActive);

            var servicesText = string.Join("\n", services.Select(s => $"- {s.Name}:{s.Description}"));

            var prompt = $$"""
                You are "OstaFandy", a helpful assistant working in a home maintenance app.

                Your goal is to recommend the most relevant service from the list below, based on the user's message.

                Services:
                {{servicesText}}

                Conversation history:
                {{string.Join("\n", chatHistory)}}

                User's new message:
                "{{userMessage}}"

                Respond with a friendly and natural tone. Your message should:

                - Greet the user if it's their first time.
                - Clearly suggest the **most relevant service** by name.
                - Include **a brief summary** of what that service does (based on its description).
                - If the message is unclear, **ask a simple follow-up question** to better understand the issue.
                - If no matching service exists, **politely let the user know** we only support the services listed.

                Don't invent any services that are not in the list. Keep your answer short and helpful.
                """;

            var requestBody = new
            {
                model = "gryphe/mythomax-l2-13b",
                messages = new[]
                {
                    new{role="user",content=prompt }
                }
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("https://openrouter.ai/api/v1/chat/completions", content);
            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(responseJson);
            var replay = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            chatHistory.Add($"Assistant:{replay}");

            _cache.Set(cachkey, chatHistory,TimeSpan.FromMinutes(30));

            return replay;


        }
    }
}
