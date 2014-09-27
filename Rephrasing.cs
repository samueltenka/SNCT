using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using BingSynonyms;     //BingSynonymsContainer
using Microsoft;        //TranslatorContainer

namespace SNCT_Pract
{
    class Program
    {
        static void Main(string[] args)
        {        
            char[] delimiterChars = {' ', '?'};
            string input = Console.ReadLine();
            List<string> inputWords = new List<string>();
            inputWords.AddRange(input.Split(delimiterChars));

            List<string> rephrases = new List<string>();

            for (int i = 0; i < inputWords.Count; i++)
            {
                if (!String.IsNullOrEmpty(inputWords[i]))
                {
                    rephrases.Add(inputWords[i]);
                    rephrases.AddRange(generalSynon(inputWords[i]));
                    //rephrases.AddRange(specialSynon(line));//FIX ME: Throw only people, products, location.
                }
            }

            for (int i = 0; i < rephrases.Count; i++)
            {
                Console.WriteLine(rephrases[i]);
            }

        }

        class BingSynonyms
        {
            private const string User_ID = "wkdxownd1007@gmail.com";
            private const string SECURE_ACCOUNT_ID = "XFOoQ3ViyZxhFruLmewr3fbqzraakvNQtfcLff3y7uo";
            private const string ROOT_SERVICE_URL = "https://api.datamarket.azure.com/Bing/Synonyms/v1/";

            private Uri serviceUri;
            private BingSynonymsContainer context;

            public BingSynonyms()
            {
                serviceUri = new Uri(ROOT_SERVICE_URL);
                context = new BingSynonymsContainer(serviceUri);
                context.IgnoreMissingProperties = true;
                context.Credentials = new NetworkCredential(User_ID, SECURE_ACCOUNT_ID);
            }

            public DataServiceQuery<GetSynonymsEntitySet> GetSynonyms(String Query)
            {
                if ((Query == null))
                {
                    throw new System.ArgumentNullException("Query", "Query value cannot be null");
                }
                DataServiceQuery<GetSynonymsEntitySet> query;
                query = context.CreateQuery<GetSynonymsEntitySet>("GetSynonyms");
                if ((Query != null))
                {
                    query = query.AddQueryOption("Query", string.Concat("\'", System.Uri.EscapeDataString(Query), "\'"));
                }
                return query;
            }

        }

        //Returns a list of synonyms of specialWord(people, location, products).
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

        //Returns a paraphrased sentences from Eng-Eng translator.// FIX ME
        static List<string> paraphrase(string question)
        {
            List<string> paraphrases = new List<string>();
                       
            return paraphrases;
        }

        //Returns synonyms for general words.
        static List<string> generalSynon(string word)
        {
            string url = string.Format("http://thesaurus.com/search?q={0}", word);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            List<string> synonyms = new List<string>();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string line;
                
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    while (((line = reader.ReadLine()) != null) && !line.Contains("synonyms-horizontal-divider"))
                    {
                        if (line.Contains("common-word"))
                        {
                            while (!line.Contains("<span class=\"text\">"))
                            {
                                line = reader.ReadLine();
                            }
                            var index = line.IndexOf("<span class=\"text\">");
                            index = index + "<span class=\"text\">".Length;
                            synonyms.Add(line.Substring(index).Replace("</span>", ""));
                        }                        
                    }
                }                
            }
            return synonyms;
        }
    }
}
