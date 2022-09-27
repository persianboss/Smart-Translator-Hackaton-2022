using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;
using TranslatorExtension.CognetiveService.Model;

namespace TranslatorExtension.CognetiveService
{
    public class TranslationService : ITranslationService
    {
        private static readonly string key = "<<your key>>";
        private static readonly string endpoint = "https://api.cognitive.microsofttranslator.com";
        private static readonly string location = "<<your region>>";
        public IHttpClientFactory _httpClientFactory { get; }

        public TranslationService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        public async Task<string> TranslateToEnglishAsync(string textToTranslate, string targetLanguage)
        {
            // Output languages are defined as parameters, input language detected.
            string route = $"/translate?api-version=3.0&to={targetLanguage.ToLower()}";
            //string textToTranslate = "Halo, rafiki! Ulifanya nini leo?";
            return await GetTranslatedMessage(textToTranslate, route).ConfigureAwait(false);
        }

        private async Task<string> GetTranslatedMessage(string textToTranslate, string route)
        {
            object[] body = new object[] { new { Text = textToTranslate } };
            var requestBody = JsonConvert.SerializeObject(body);
            var httpRequestMessage = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri(endpoint + route))
            {
                Headers =
                        {
                            {"Ocp-Apim-Subscription-Key", key },
                            {"Ocp-Apim-Subscription-Region", location }
                        },
                Content = new StringContent(requestBody, Encoding.UTF8, "application/json")
            };
            var httpClient = _httpClientFactory.CreateClient();
            var responseMessage = await httpClient.SendAsync(httpRequestMessage).ConfigureAwait(false);
            return await responseMessage.Content.ReadAsStringAsync();
        }
    }
}
