using Newtonsoft.Json;
using System;

namespace MyWebApplication.RecaptchaV3
{
    public class RecaptchaV3VerifyResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("score")]
        public decimal Score { get; set; }

        [JsonProperty("action")]
        public string Action { get; set; }

        [JsonProperty("hostname")]
        public string Hostname { get; set; }

        [JsonProperty("challenge_ts")]
        public DateTime ChallengeTs { get; set; }

        [JsonProperty("error-codes")]
        public string[] ErrorCodes { get; set; }

        // NON arriva da Google: la settiamo noi per debug/log
        [JsonIgnore]
        public string RawResponse { get; set; }
    }

}