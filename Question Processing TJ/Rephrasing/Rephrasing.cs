using System;
using System.IO;
using System.Net;
using System.Text;
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
        static void Main(string[] args)
        {
            string question = Console.ReadLine();
            string paraphraseOutput = paraphrasing(question, '5');
            string[] paraphrases
                = paraphraseOutput.Substring(paraphraseOutput.IndexOf("[")).
                Split(new char[] { ',', ':', '\"', '[', ']', '}', '?' });
            List<string> processed = new List<string>();
            foreach (string paraphrase in paraphrases)
            {
                if (paraphrase != "")
                {
                    processed.Add(paraphrase);
                }
            }

            foreach (string proce in processed)
            {
                Console.WriteLine(proce);
            }
        }

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
                    "XFOoQ3ViyZxhFruLmewr3fbqzraakvNQtfcLff3y7uo");
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