using Easy.Net.Starter.Exceptions;
using Newtonsoft.Json;

namespace Easy.Net.Starter.Security
{
    public static class ReCaptchaTokenValidator
    {
        public static async Task ValidationReCaptchaAsync(string token, string secret)
        {
            var dictionary = new Dictionary<string, string>
                    {
                        { "secret", secret },
                        { "response", token }
                    };

            var postContent = new FormUrlEncodedContent(dictionary);

            HttpResponseMessage recaptchaResponse = null;
            string stringContent = "";

            // Call recaptcha api and validate the token
            using (var http = new HttpClient())
            {
                recaptchaResponse = await http.PostAsync("https://www.google.com/recaptcha/api/siteverify", postContent);
                stringContent = await recaptchaResponse.Content.ReadAsStringAsync();
            }

            if (!recaptchaResponse.IsSuccessStatusCode)
            {
                throw new BusinessException("Accès non autorisé", "Accès non autorisé", "403");
            }

            if (string.IsNullOrEmpty(stringContent))
            {
                throw new BusinessException("Accès non autorisé", "Accès non autorisé", "403");
            }

            var googleReCaptchaResponse = JsonConvert.DeserializeObject<ReCaptchaResponse>(stringContent);

            if (!googleReCaptchaResponse.Success)
            {
                var errors = string.Join(",", googleReCaptchaResponse.ErrorCodes);

                Console.WriteLine(errors);

                throw new BusinessException("Accès non autorisé", "Accès non autorisé", "403");
            }

            if (!googleReCaptchaResponse.Action.Equals("preInscriptionAction", StringComparison.OrdinalIgnoreCase))
            {
                throw new BusinessException("Accès non autorisé", "Accès non autorisé", "403");
            }

            if (googleReCaptchaResponse.Score < 0.5)
            {
                throw new BusinessException("Accès non autorisé", "Accès non autorisé", "403");
            }
        }
    }
}
