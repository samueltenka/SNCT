using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.Web.Http;
using Windows.Data.Json;

namespace SNCT
{
    public class AlchemyProvider
    {
        private const string ALCHEMY_API_KEY = "b07bf5d8641c359b725f87c507d9f7554c32849b";

        public static async Task<string> URLGetText(string url)
        {
            string apiEndpoint = "http://access.alchemyapi.com/calls/url/URLGetText";
            apiEndpoint += "?outputMode=json&apikey=" + ALCHEMY_API_KEY + "&url=" + System.Uri.EscapeUriString(url);
            string cleanedText;
            using (var client = new HttpClient())
            {
                string response = await client.GetStringAsync(new Uri(apiEndpoint));
                JsonObject obj = JsonObject.Parse(response);
                cleanedText = obj.GetNamedString("text");
            }
            return cleanedText;
        }
    }

}
