using Newtonsoft.Json;
using System.Collections.Generic;

namespace Simco.Models
{
    public class CaptchaResponse
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error-codes")]
        public List<string> ErrorMessage { get; set; }
    }
}