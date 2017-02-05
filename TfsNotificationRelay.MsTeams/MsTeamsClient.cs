using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DevCore.TfsNotificationRelay.MsTeams.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DevCore.TfsNotificationRelay.MsTeams
{
    class MsTeamsClient:HttpClient
    {
        private const string mediaType = "application/json";

        private JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        private string webHookURL;
        public MsTeamsClient(string webHookURL)
        {
            this.webHookURL = webHookURL;
        }

        public async Task<HttpResponseMessage> SendWebhookMessageAsync(Message message)
        {
            var content = await GetContent(message);
            return await PostAsync(webHookURL, content);
        }



        private Task<StringContent> GetContent(Message message)
        {
            return Task.Factory.StartNew(() =>
            {
                string json = JsonConvert.SerializeObject(message, Formatting.None, SerializerSettings);
                var content = new StringContent(json, Encoding.UTF8, mediaType);
                return content;
            });
        }


    }
}
