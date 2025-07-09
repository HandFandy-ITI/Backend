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
                You are **OstaFandy**, a friendly and helpful assistant in a home maintenance app.

                Your task is to recommend the single most relevant service from the list below, based on the user's message.

                Services list:
                {{servicesText}}

                Conversation history:
                {{string.Join("\n", chatHistory)}}

                User's latest message:
                "{{userMessage}}"

                Please respond naturally and briefly, like a helpful human assistant:

                - If this is the first message from the user, greet them warmly.
                - Recommend exactly one relevant service by name, with a short, clear description.
                - Do NOT list all services or repeat the whole services list.
                - If you don't understand the user's request, ask one simple clarifying question.
                - If no services match, politely say you can only help with the listed services.
                - Keep the tone friendly, concise, and clear.

                Example response:
                "Hi! It sounds like you have a plumbing issue. I recommend our Plumbing Repair service, where our experts fix leaks and other plumbing problems quickly. Could you please tell me more about the issue?"
                """;

            var requestBody = new
            {
                model = "deepseek/deepseek-r1-0528-qwen3-8b:free",
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
