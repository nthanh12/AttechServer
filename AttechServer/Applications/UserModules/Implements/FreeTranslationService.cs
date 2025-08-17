using AttechServer.Applications.UserModules.Abstracts;
using System.Text.Json;

namespace AttechServer.Applications.UserModules.Implements
{
    public class FreeTranslationService : ITranslationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<FreeTranslationService> _logger;

        public FreeTranslationService(HttpClient httpClient, ILogger<FreeTranslationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
            _httpClient.Timeout = TimeSpan.FromSeconds(10);
        }

        public async Task<string> TranslateTextAsync(string text, string sourceLanguage, string targetLanguage)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            try
            {
                // Option 1: LibreTranslate (free, no API key needed)
                return await TranslateWithLibreTranslate(text, sourceLanguage, targetLanguage);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"LibreTranslate failed: {ex.Message}");

                try
                {
                    // Option 2: MyMemory (backup free service)
                    return await TranslateWithMyMemory(text, sourceLanguage, targetLanguage);
                }
                catch (Exception ex2)
                {
                    _logger.LogWarning($"MyMemory failed: {ex2.Message}");

                    // Option 3: Simple word replacement fallback
                    return SimpleTranslateFallback(text, sourceLanguage, targetLanguage);
                }
            }
        }

        private async Task<string> TranslateWithLibreTranslate(string text, string source, string target)
        {
            var url = "https://libretranslate.de/translate";
            var payload = new
            {
                q = text,
                source = source,
                target = target,
                format = "text"
            };

            var response = await _httpClient.PostAsJsonAsync(url, payload);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

            return result.GetProperty("translatedText").GetString() ?? text;
        }

        private async Task<string> TranslateWithMyMemory(string text, string source, string target)
        {
            var langPair = $"{source}|{target}";
            var encodedText = Uri.EscapeDataString(text);
            var url = $"https://api.mymemory.translated.net/get?q={encodedText}&langpair={langPair}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

            return result.GetProperty("responseData")
                        .GetProperty("translatedText")
                        .GetString() ?? text;
        }

        private string SimpleTranslateFallback(string text, string source, string target)
        {
            if (source == "vi" && target == "en")
            {
                var translations = new Dictionary<string, string>
                {
                    {"tin tức", "news"},
                    {"sản phẩm", "product"},
                    {"dịch vụ", "service"},
                    {"danh mục", "category"},
                    {"thông báo", "notification"},
                    {"mô tả", "description"},
                    {"tên", "name"},
                    {"tiêu đề", "title"},
                    {"nội dung", "content"},
                    {"hình ảnh", "image"},
                    {"trạng thái", "status"},
                    {"hoạt động", "active"},
                    {"không hoạt động", "inactive"}
                };

                var result = text.ToLower();
                foreach (var kvp in translations)
                {
                    result = result.Replace(kvp.Key, kvp.Value);
                }
                return result;
            }

            return text;
        }
    }
}
