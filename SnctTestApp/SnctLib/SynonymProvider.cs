using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Windows.Foundation;
using Windows.Web.Http;
using Windows.Data.Json;

namespace SNCT
{
    public class SynonymResult
    {
        public string Title { get; set; }

        public SynonymResult(string title)
        {
            this.Title = title;
        }
    }

    public class SynonymProvider
    {
        private const string API_URI = "https://api.datamarket.azure.com/Bing/Synonyms/v1/";
        private const string API_KEY = "jcdA+vAHn5CA0FT7q982djnUStUHzOdn92Pne9eslwc";

        public static async Task<ObservableCollection<SynonymResult>> GetResultsForQuery(string query)
        {
            string queryParams = "GetSynonyms?Query=%27" + query + "%27&$format=json";
            string searchUri = API_URI + queryParams;
            var filter = new Windows.Web.Http.Filters.HttpBaseProtocolFilter();
            filter.AllowUI = false;
            filter.ServerCredential = new Windows.Security.Credentials.PasswordCredential(searchUri, "empty", API_KEY);
            JsonArray jsonResults;
            using (var client = new HttpClient(filter))
            {
                string response = await client.GetStringAsync(new Uri(searchUri));
                JsonObject obj = JsonObject.Parse(response);
                jsonResults = obj.GetNamedObject("d").GetNamedArray("results");
            }

            var results = new ObservableCollection<SynonymResult>();
            for(uint i = 0; i < jsonResults.Count; i++) {
                var obj = jsonResults.GetObjectAt(i);
                results.Add(new SynonymResult(obj.GetNamedString("Synonym")));
            }
            return results;
        }
    }
}
