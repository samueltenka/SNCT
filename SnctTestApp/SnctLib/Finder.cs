using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SNCT
{
    public class Finder
    {
        async public static Task<SortedDictionary<String, double>> answer(String text, String[] queries, int num)
        {
            HashSet<String> query_content_words = new HashSet<String>();
            foreach (String q in queries)
            {
                foreach (String word in q.Split())
                {
                    query_content_words.Add(q);
                }
            }

            PhraseGraph PG = await PhraseGraph.build_PG(text, queries[0], query_content_words);
            for (int i = 0; i < 200; ++i)
            {
                PG.step(0.05);
            }

            return PG.get_top(num);
        }
    }
}
