using DeepL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace IniTranslator.Helpers
{
    internal class Translators
    {
        /// <summary>
        /// Translates text using the Google Translate API.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="targetLanguage"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static async Task<string> GoogleTranslate(string text, string targetLanguage, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                return text;
            using HttpClient client = new HttpClient();
            string url = $"https://translation.googleapis.com/language/translate/v2?key={apiKey}";

            // Prepare the request body
            var requestBody = new
            {
                q = text,
                target = targetLanguage,
                format = "text"
            };
            var content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

            // Make the POST request
            HttpResponseMessage response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error translating text: {response.StatusCode} - {response.ReasonPhrase}");
            }

            // Parse the response
            string responseBody = await response.Content.ReadAsStringAsync();
            JsonDocument jsonResponse = JsonDocument.Parse(responseBody);
            string? translatedText = jsonResponse.RootElement.GetProperty("data").GetProperty("translations")[0].GetProperty("translatedText").GetString();
            if (translatedText is null)
                return text;
            return translatedText;
        }

        /// <summary>
        /// Translates text using the DeepL API.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="targetLanguage"></param>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        public static async Task<string> DeepLTranslate(string text, string targetLanguage, string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
                return text;
            var translator = new Translator(apiKey);
            var options = new TextTranslateOptions
            {
                Context = "Traslations for Star Citizen in a `key=value` format. The translation will focus only on the text after the `=` sign, preserving the original format and treating `\\n` characters as literal text, not line breaks. Names of people, groups, and organizations will remain unchanged. The translation will use a neutral tone suitable for a gaming audience."
            };
            var translatedText = await translator.TranslateTextAsync(text, LanguageCode.English, targetLanguage, options);
            return translatedText.Text;
        }
    }
}
