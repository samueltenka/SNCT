using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Web;

namespace SNCT_Pract
{
    class Program
    {
        static void Main(string[] args)
        {

            string input;
            StreamReader reader = new StreamReader("dic.txt");
            input = reader.ReadLine();

            for (char i = 'a'; i <= 'z'; i++)
            {
                for (char j = 'a'; j <= 'z'; j++)
                {
                    StreamWriter writer = new StreamWriter("dic-" + i + "-" + j + ".txt");

                    while (input[0] == i && (input.Length == 1 || (input[1] == j)))
                    {
                        writer.WriteLine("-----------word----------");
                        writer.WriteLine(input);
                        generalSynon(input, ref writer);
                        input = reader.ReadLine();
                    }

                    writer.Close();
                }
            }

            reader.Close();

            /*
            string input;
            input = Console.ReadLine();
            StreamWriter writer = new StreamWriter("test.txt");
            generalSynon(input, ref writer);
            writer.Close();      
            */
        }

        //Returns synonyms for general words.
        static void generalSynon(string word, ref StreamWriter writer)
        {
            string url = string.Format("http://thesaurus.com/search?q={0}", word);

            WebRequest request = WebRequest.Create(url);
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Proxy = null;
            WebResponse response = request.GetResponse();
            List<string> synonyms = new List<string>();
            ServicePointManager.Expect100Continue = false;


            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                string line = reader.ReadLine();
                while (line != null)
                {
                    if (line != null && line.Contains("synonyms-heading"))
                    {
                        line = reader.ReadLine();
                        var index = line.IndexOf("<em class=\"text\">");
                        index += "<em class=\"text\">".Length * 2 - 1;
                        writer.WriteLine("--------word type--------");
                        writer.WriteLine(line.Substring(index).Replace("</em>", ""));
                        writer.WriteLine("--------synonyms---------");
                        while (line != null && !line.Contains("synonyms-horizontal-divider"))
                        {
                            if (line != null && line.Contains("common-word"))
                            {
                                while (line != null && !line.Contains("<span class=\"text\">"))
                                {
                                    line = reader.ReadLine();
                                }
                                index = line.IndexOf("<span class=\"text\">");
                                index += "<span class=\"text\">".Length;
                                writer.WriteLine(line.Substring(index).Replace("</span>", ""));
                            }
                            line = reader.ReadLine();
                        }
                    }
                    line = reader.ReadLine();
                }
            }
        }
    }
}
