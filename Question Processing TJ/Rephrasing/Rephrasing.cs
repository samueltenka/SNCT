using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using BingSynonyms;         //BingSynonymsContainer

namespace QuestionRephraseAndSynonyms
{
    class Program
    {
        private const string AccountKey = "XFOoQ3ViyZxhFruLmewr3fbqzraakvNQtfcLff3y7uo";

       // private string _plainText;

        static void Main(string[] args)
        {

            string question = Console.ReadLine();
            List<string> paraList = new List<string>();
            paraListGenerator(question, paraList);
            foreach (string proce in paraList)
            {
                Console.WriteLine(proce);
            }


            string question = Console.ReadLine();
            Console.WriteLine("Web Result---------------------------------");
            try
            {
                MakeRequest(question);
            }
            catch (Exception ex)
            {
                string innerMessage =
                    (ex.InnerException != null) ?
                    ex.InnerException.Message : String.Empty;
                Console.WriteLine("{0}\n{1}", ex.Message, innerMessage);
            }
            Console.WriteLine("\nNews Result---------------------------------");
            try
            {
                MakeNewsRequest(question);
            }
            catch (Exception ex)
            {
                string innerMessage =
                    (ex.InnerException != null) ?
                    ex.InnerException.Message : String.Empty;
                Console.WriteLine("{0}\n{1}", ex.Message, innerMessage);
            }
            Console.WriteLine("\nRelatedSearch Result---------------------------------");
            try
            {
                MakeRelatedRequest(question);
            }
            catch (Exception ex)
            {
                string innerMessage =
                    (ex.InnerException != null) ?
                    ex.InnerException.Message : String.Empty;
                Console.WriteLine("{0}\n{1}", ex.Message, innerMessage);
            }
         
            /*
            // Create an AlchemyAPI object.
            AlchemyAPI.AlchemyAPI alchemyObj = new AlchemyAPI.AlchemyAPI();


            // Load an API key from disk.
            alchemyObj.LoadAPIKey("api_key.txt");


            // Extract page text from a web URL. (ignoring ads, navigation links, etc.)
            string xml = alchemyObj.URLGetText("http://environment.nationalgeographic.com/environment/photos/megafishes-gallery/");
            Console.WriteLine(xml);


            // Extract raw page text from a web URL. (including ads, navigation links, etc.)
            xml = alchemyObj.URLGetRawText("http://environment.nationalgeographic.com/environment/photos/megafishes-gallery/");
            Console.WriteLine(xml);


            // Extract a title from a web URL.
            xml = alchemyObj.URLGetTitle("http://environment.nationalgeographic.com/environment/photos/megafishes-gallery/");
            Console.WriteLine(xml);
            */
            /*
            // Load a HTML document to analyze.
            StreamReader streamReader = new StreamReader("data/example.html");
            string htmlDoc = streamReader.ReadToEnd();
            streamReader.Close();


            // Extract page text from a HTML document. (ignoring ads, navigation links, etc.)
            xml = alchemyObj.HTMLGetText(htmlDoc, "http://www.test.com/");
            Console.WriteLine(xml);


            // Extract raw page text from a HTML document. (including ads, navigation links, etc.)
            xml = alchemyObj.HTMLGetRawText(htmlDoc, "http://www.test.com/");
            Console.WriteLine(xml);


            // Extract a title from a HTML document.
            xml = alchemyObj.HTMLGetTitle(htmlDoc, "http://www.test.com/");
            Console.WriteLine(xml);
            */

        }
    
        //Bing Search
        static void MakeRequest(string question)
        {
            // This is the query expression.
            string query = question;

            // Create a Bing container.
            string rootUrl = "https://api.datamarket.azure.com/Bing/Search";
            var bingContainer = new Bing.BingSearchContainer(new Uri(rootUrl));

            // The market to use.
            string market = "en-us";

            // Configure bingContainer to use your credentials.
            bingContainer.Credentials = new NetworkCredential(AccountKey, AccountKey);

            // Build the query, limiting to 10 results.
            var webQuery =
                bingContainer.Web(query, null, null, market, null, null, null, null);
            webQuery = webQuery.AddQueryOption("$top", 10);

            // Run the query and display the results.
            var webResults = webQuery.Execute();

            foreach (var result in webResults)
            {
                Console.WriteLine("{0}\n\t{1}", result.Title, result.Url);
            }
        }

        static void MakeNewsRequest(string question)
        {
            // This is the query expression.
            string query = question;

            // Create a Bing container.

            string rootUrl = "https://api.datamarket.azure.com/Bing/Search";

            var bingContainer = new Bing.BingSearchContainer(new Uri(rootUrl));

            // The market to use.

            string market = "en-us";

            // Get news for science and technology.

            string newsCat = "rt_ScienceAndTechnology";

            // Configure bingContainer to use your credentials.

            bingContainer.Credentials = new NetworkCredential(AccountKey, AccountKey);

            // Build the query, limiting to 10 results.

            var newsQuery =

            bingContainer.News(query, null, market, null, null, null, null, newsCat, null);

            newsQuery = newsQuery.AddQueryOption("$top", 10);

            // Run the query and display the results.

            var newsResults = newsQuery.Execute();

            foreach (var result in newsResults)
            {

                Console.WriteLine("{0}-{1}\n\t{2}",

                result.Source, result.Title, result.Description);

            }
        }

        static void MakeRelatedRequest(string question)
        {
            // This is the query expression.
            string query = question;

            // Create a Bing container.

            string rootUrl = "https://api.datamarket.azure.com/Bing/Search";

            var bingContainer = new Bing.BingSearchContainer(new Uri(rootUrl));

            // The market to use.

            string market = "en-us";

            // Configure bingContainer to use your credentials.

            bingContainer.Credentials = new NetworkCredential(AccountKey, AccountKey);

            // Build the query, limiting to 5 results.

            var relatedQuery =

            bingContainer.RelatedSearch(query, null, market, null, null, null);

            relatedQuery = relatedQuery.AddQueryOption("$top", 5);

            // Run the query and display the results.

            var relatedResults = relatedQuery.Execute();

            foreach (var result in relatedResults)
            {

                Console.WriteLine("{0}\n\t{1}", result.Title, result.BingUrl);

            }
        }

        static void paraListGenerator(string question, List<string> paraList)
        {
            string paraAPIOut = paraphrasing(question, '5');
            string[] paraAPIOutSpl
                = paraAPIOut.Substring(paraAPIOut.IndexOf("[")).
                Split(new char[] { ',', ':', '\"', '[', ']', '}', '?' });

            foreach (string para in paraAPIOutSpl)
            {
                if (para != "")
                {
                    paraList.Add(para);
                }
            }
        }
        //Bing Syn
        class BingSynonyms
        {
            private const string User_ID = "wkdxownd1007@gmail.com";
            private const string SECURE_ACCOUNT_ID
                = "XFOoQ3ViyZxhFruLmewr3fbqzraakvNQtfcLff3y7uo";
            private const string ROOT_SERVICE_URL 
                = "https://api.datamarket.azure.com/Bing/Synonyms/v1/";

            private Uri serviceUri;
            private BingSynonymsContainer context;

            public BingSynonyms()
            {
                serviceUri = new Uri(ROOT_SERVICE_URL);
                context = new BingSynonymsContainer(serviceUri);
                context.IgnoreMissingProperties = true;
                context.Credentials
                    = new NetworkCredential(User_ID, SECURE_ACCOUNT_ID);
            }

            public DataServiceQuery<GetSynonymsEntitySet>
                GetSynonyms(String Query)
            {
                if ((Query == null))
                {
                    throw new System.
                        ArgumentNullException("Query",
                        "Query value cannot be null");
                }
                DataServiceQuery<GetSynonymsEntitySet> query;
                query
                    = context.CreateQuery<GetSynonymsEntitySet>("GetSynonyms");
                if ((Query != null))
                {
                    query = query.
                        AddQueryOption("Query", string.
                        Concat("\'", System.
                        Uri.EscapeDataString(Query), "\'"));
                }
                return query;
            }

        }
        //Para
        private static string GetParaphrases(string url, string headerValue)
        {
            string responseContent = string.Empty;

            using (HttpWebResponse webResponse
                = InvokeRestGetService(url, headerValue, null))
            {
                //extract response string from response
                //and do further validations
                using (StreamReader streamResponse
                    = new StreamReader(webResponse.GetResponseStream()))
                {
                    responseContent = streamResponse.ReadToEnd();
                }
            }

            return responseContent;
        }
        //Para
        private static HttpWebResponse InvokeRestGetService(string url,
            string headerValue, IWebProxy proxy)
        {
            // build HTTP request
            HttpWebRequest restWebRequest = null;

            restWebRequest = (HttpWebRequest)WebRequest.Create(url);
            restWebRequest.Headers.Add("Authorization", headerValue);
            // If we have a proxy set it to avoid 503s
            if (proxy != null)
            {
                restWebRequest.Proxy = proxy;
            }
            restWebRequest.Method = "GET";

            // return HttpWebResponse
            return (HttpWebResponse)restWebRequest.GetResponse();
        }
        //Para
        static string paraphrasing(string sentence, char numOfParas)
        {
            AdmAccessToken admToken;

            string urlTemplate
                = "http://api.microsofttranslator.com/v3/json/paraphrase?sentence={0}&language={1}&category={2}&maxParaphrases={3}";
            string headerValue;
            //Get Client Id and Client Secret from
            //https://datamarket.azure.com/developer/applications/
            AdmAuthentication admAuth
                = new AdmAuthentication(
                    "61ac4bbd-ceea-4cd2-a33e-26ac7644f516",
                    "I7kv+trhQqTDI9egay1fwTgXG0URzMnb4m4kURzov5k=");
            try
            {
                admToken = admAuth.GetAccessToken();
                DateTime tokenReceived = DateTime.Now;
                // Create a header with the access_token
                //property of the returned token
                headerValue = "Bearer " + admToken.access_token;
                if ((DateTime.Now - tokenReceived).TotalSeconds > 600)
                {
                    throw new Exception("Authentication token expired");
                }
                else
                {
                    string url = string.Format(urlTemplate,
                        sentence, "en-us", "general", numOfParas);
                    string paraphraseResult = GetParaphrases(url, headerValue);
                    return paraphraseResult;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        //Bing Syn
        //Returns a list of synonyms of
        //specialWord(people, location, products).
        static List<string> specialSynon(string specialWord)
        {
            DataServiceQuery<GetSynonymsEntitySet> SynonymsDataServiceQuery;
            BingSynonyms bingSynonyms = new BingSynonyms();
            List<string> synonyms = new List<string>();

            SynonymsDataServiceQuery = bingSynonyms.GetSynonyms(specialWord);
            foreach (GetSynonymsEntitySet entity in SynonymsDataServiceQuery)
            {
                synonyms.Add(entity.Synonym);
            }
            return synonyms;
        }
    }
    //Para
    [DataContract]
    public class AdmAccessToken
    {
        [DataMember]
        public string access_token { get; set; }
        [DataMember]
        public string token_type { get; set; }
        [DataMember]
        public string expires_in { get; set; }
        [DataMember]
        public string scope { get; set; }
    }
    //Para
    public class AdmAuthentication
    {
        public static readonly string DatamarketAccessUri
            = "https://datamarket.accesscontrol.windows.net/v2/OAuth2-13";
        private string clientId;
        private string cientSecret;
        private string request;

        public AdmAuthentication(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.cientSecret = clientSecret;
            //If clientid or client secret has special characters,
            //encode before sending request
            this.request = string.Format(
                "grant_type=client_credentials&client_id={0}&client_secret={1}&scope=http://api.microsofttranslator.com",
                HttpUtility.UrlEncode(clientId),
                HttpUtility.UrlEncode(clientSecret));
        }

        public AdmAccessToken GetAccessToken()
        {
            return HttpPost(DatamarketAccessUri, this.request);
        }

        private AdmAccessToken HttpPost(string DatamarketAccessUri,
            string requestDetails)
        {
            //Prepare OAuth request 
            WebRequest webRequest = WebRequest.Create(DatamarketAccessUri);
            webRequest.ContentType = "application/x-www-form-urlencoded";
            webRequest.Method = "POST";
            byte[] bytes = Encoding.ASCII.GetBytes(requestDetails);
            webRequest.ContentLength = bytes.Length;
            using (Stream outputStream = webRequest.GetRequestStream())
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }
            using (WebResponse webResponse = webRequest.GetResponse())
            {
                DataContractJsonSerializer serializer
                    = new DataContractJsonSerializer(typeof(AdmAccessToken));
                //Get deserialized object from JSON stream
                AdmAccessToken token
                    = (AdmAccessToken)serializer.
                    ReadObject(webResponse.GetResponseStream());
                return token;
            }
        }
    }
}
