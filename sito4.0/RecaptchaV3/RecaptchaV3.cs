using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace MyWebApplication.RecaptchaV3
{
    public static class RecaptchaV3
    {

        public static async Task<RecaptchaV3VerifyResponse> VerifyRequestAsync(string token, string userIp = null)
        {
            var secret = ConfigurationManager.AppSettings["RecaptchaV3.SecretKey"];
            if (string.IsNullOrWhiteSpace(secret))
                throw new ConfigurationErrorsException("RecaptchaV3.SecretKey mancante in web.config");

            if (string.IsNullOrWhiteSpace(token))
                return new RecaptchaV3VerifyResponse { Success = false, RawResponse = "Token vuoto" };

            // Su hosting legacy a volte serve TLS 1.2 esplicito
            // (in genere su .NET 4.6.2 è già ok, ma dipende da config/OS)
            System.Net.ServicePointManager.SecurityProtocol |= System.Net.SecurityProtocolType.Tls12;

            using (var http = new HttpClient())
            {
                http.Timeout = TimeSpan.FromSeconds(10);
                http.DefaultRequestHeaders.Accept.Clear();
                http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var values = new Dictionary<string, string>
                {
                    { "secret", secret },
                    { "response", token }
                };
                if (!string.IsNullOrWhiteSpace(userIp))
                    values.Add("remoteip", userIp);

                try
                {
                    using (var content = new FormUrlEncodedContent(values))
                    using (var resp = await http.PostAsync("https://www.google.com/recaptcha/api/siteverify", content).ConfigureAwait(false))
                    {
                        var body = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);
                        var contentType = resp.Content.Headers.ContentType?.MediaType;

                        // Se non è 200, logga body per capire cosa arriva (proxy/WAF/etc.)
                        if (!resp.IsSuccessStatusCode)
                        {
                            return new RecaptchaV3VerifyResponse
                            {
                                Success = false,
                                RawResponse = $"HTTP {(int)resp.StatusCode} {resp.ReasonPhrase} | Content-Type={contentType} | Body={body}"
                            };
                        }

                        // A volte arrivano risposte non JSON (proxy). Le catturiamo.
                        if (string.IsNullOrWhiteSpace(body) || body.TrimStart().StartsWith("<"))
                        {
                            return new RecaptchaV3VerifyResponse
                            {
                                Success = false,
                                RawResponse = $"Risposta non JSON | Content-Type={contentType} | Body={body}"
                            };
                        }

                        var parsed = JsonConvert.DeserializeObject<RecaptchaV3VerifyResponse>(body)
                                     ?? new RecaptchaV3VerifyResponse { Success = false };

                        parsed.RawResponse = body;
                        return parsed;
                    }
                }
                catch (Exception ex)
                {
                    return new RecaptchaV3VerifyResponse
                    {
                        Success = false,
                        RawResponse = ex.ToString()
                    };
                }
            }
        }




        public static bool VerifyResponse(RecaptchaV3VerifyResponse verify, string expectedAction, out string errorMessage)
        {
            if (!verify.Success)
            {
                errorMessage = "Verifica anti-bot non valida. Riprova.";
                return false;

            }

            if (!string.Equals(verify.Action, expectedAction, StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = "Verifica anti-bot non coerente. Riprova.";
                return false;
            }

            var minScoreStr = ConfigurationManager.AppSettings["RecaptchaV3.MinScore"] ?? "0.5";
            decimal minScore = 0.5m;
            decimal.TryParse(minScoreStr, NumberStyles.Any, CultureInfo.InvariantCulture, out minScore);


            if (verify.Score < minScore)
            {
                errorMessage = "Verifica anti-bot insufficiente. Riprova.";
                return false;
            }


            // Consigliato: controlla che l'hostname combaci (adatta ai tuoi domini)
            if (!string.Equals(verify.Hostname, "trova-libro.it", StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(verify.Hostname, "localhost", StringComparison.OrdinalIgnoreCase) &&
                 !string.Equals(verify.Hostname, "www.trova-libro.it", StringComparison.OrdinalIgnoreCase))
            {
                errorMessage = "Verifica CAPTCHA non valida (hostname).";
                return false;
            }
            errorMessage = "";
            return true;
        }

    }
}