using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.Web.Http;
using Windows.Data.Json;

namespace SNCT
{
    public class Rephrasing
    {
        class AdmAccessToken
        {
            public string access_token { get; set; }
            public string token_type { get; set; }
            public string expires_in { get; set; }
            public string scope { get; set; }
        }

        class AdmAuthentication
        {
            public static readonly string DatamarketAccessUri = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
            private string clientId;
            private string clientSecret;

            public AdmAuthentication(string clientId, string clientSecret)
            {
                this.clientId = clientId;
                this.clientSecret = clientSecret;
            }

            async public Task<AdmAccessToken> GetAccessToken()
            {
                return await HttpPost(DatamarketAccessUri);
            }

            async private Task<AdmAccessToken> HttpPost(string DatamarketAccessUri)
            {
                using (HttpClient client = new HttpClient())
                {
                    var list = new List<KeyValuePair<string, string>>();
                    list.Add(new KeyValuePair<string,string>("grant_type", "client_credentials"));
                    list.Add(new KeyValuePair<string,string>("client_id", Uri.EscapeUriString(clientId)));
                    list.Add(new KeyValuePair<string,string>("client_secret", Uri.EscapeUriString(clientSecret)));
                    list.Add(new KeyValuePair<string, string>("scope", "http://api.microsofttranslator.com"));
                    var hscDetails = new HttpFormUrlEncodedContent(list);
                    var headers = client.DefaultRequestHeaders;
                    var httpResponse = await client.PostAsync(new Uri(DatamarketAccessUri), hscDetails);
                    var strResponse = await httpResponse.Content.ReadAsStringAsync();

                    var obj = JsonObject.Parse(strResponse);
                    AdmAccessToken token = new AdmAccessToken() {
                        access_token = obj.GetNamedString("access_token"),
                        token_type = obj.GetNamedString("token_type"),
                        expires_in = obj.GetNamedString("expires_in"),
                        scope = obj.GetNamedString("scope")
                    };
                    return token;
                }
            }
        }

        private const string AccountKey = "XFOoQ3ViyZxhFruLmewr3fbqzraakvNQtfcLff3y7uo";

        public async static Task<JsonArray> GetParaphrases(string sentence, int maxParaphrases)
        {
            string urlTemplate = "http://api.microsofttranslator.com/v3/json/paraphrase?sentence={0}&language={1}&category={2}&maxParaphrases={3}";
            string url = string.Format(urlTemplate, sentence, "en-us", "general", maxParaphrases);

            AdmAuthentication admAuth = new AdmAuthentication(
                "61ac4bbd-ceea-4cd2-a33e-26ac7644f516",
                "I7kv+trhQqTDI9egay1fwTgXG0URzMnb4m4kURzov5k=");

            AdmAccessToken token = await admAuth.GetAccessToken();

            string responseContent = string.Empty;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var headers = client.DefaultRequestHeaders;
                    headers.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("Bearer", token.access_token);
                    responseContent = await client.GetStringAsync(new Uri(url));
                }
                catch (Exception ex)
                {
                    responseContent = ex.Message;
                }
            }
            var obj = JsonObject.Parse(responseContent);
            JsonArray paraphrases = obj.GetNamedArray("paraphrases");
            return paraphrases;
        }
    }
}
