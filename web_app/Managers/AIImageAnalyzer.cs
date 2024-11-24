namespace Cassini.Managers
{
    public class AIImageAnalyzer
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public AIImageAnalyzer(HttpClient httpClient, string apiKey)
        {
            _httpClient = httpClient;
            _apiKey = apiKey;
        }

        public async Task<string> AnalyzeImages(string base64Image1, string base64Image2)
        {
            var requestBody = new
            {
                model = "gpt-4",
                messages = new[]
                {
                new
                {
                    role = "system",
                    content = "You are an AI model tasked with analyzing and comparing two images provided as Base64 strings."
                },
                new
                {
                    role = "user",
                    content = "Here are two satellite images. Describe what you see in each and identify the differences between them.",
                },
                new
                {
                    role = "user",
                    content = $"Image 1: {base64Image1}",
                },
                new
                {
                    role = "user",
                    content = $"Image 2: {base64Image2}",
                }
            }
            };

            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestBody), System.Text.Encoding.UTF8, "application/json");

            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _apiKey);
            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"OpenAI API call failed: {response.StatusCode}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }
    }
}

