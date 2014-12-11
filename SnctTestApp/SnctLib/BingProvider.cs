using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.Web.Http;
using Windows.Data.Json;

namespace SNCT
{
	public class WebResult {
        public string ID { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string Url { get; private set; }
        public string ArticleText { get; private set; }

        public WebResult(string id, string title, string description, string url, string articleText) {
            ID = id;
            Title = title;
            Description = description;
            Url = url;
            ArticleText = articleText;
        }
	}

	public class BingProvider {
        private const string BING_SEARCH_URI = "https://api.datamarket.azure.com/Bing/Search/";
        private const string BING_SEARCH_API_KEY = "jcdA+vAHn5CA0FT7q982djnUStUHzOdn92Pne9eslwc";

        public static async Task<ObservableCollection<WebResult>> GetResultsForQuery(string query) {
            string queryParams = "Web?Query=%27" + query + "%27&$format=json";
            string searchUri = BING_SEARCH_URI + queryParams;
            var filter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
            filter.AllowUI = false;
            filter.ServerCredential = new Windows.Security.Credentials.PasswordCredential(searchUri, "empty", BING_SEARCH_API_KEY);
            JsonArray jsonResults;
            using (var client = new HttpClient(filter))
            {
                string response = await client.GetStringAsync(new Uri(searchUri));
                JsonObject obj = JsonObject.Parse(response);
                jsonResults = obj.GetNamedObject("d").GetNamedArray("results");
            }

            var results = new ObservableCollection<WebResult>();
            for (var i = 0; i < jsonResults.Count; i++) {
                var obj = jsonResults.GetObjectAt((uint)i);
                string articleText = "";
                results.Add(new WebResult(
                    obj.GetNamedString("ID"),
                    obj.GetNamedString("Title"),
                    obj.GetNamedString("Description"),
                    obj.GetNamedString("Url"),
                    articleText
                ));
            }

            return results;
        }
    }
}
