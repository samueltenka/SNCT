using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNCT
{
    public class Finder
    {
        private static String[] function_words = "be able to can could dare had better have to may might must need to ought ought to shall should used to will would accordingly after albeit although and as because before both but consequently either for hence however if neither nevertheless nor once or since so than that then thence therefore tho' though thus till unless until when whence whenever where whereas wherever whether while whilst yet a all an another any both each either every her his its my neither no other our per some that the their these this those whatever whichever your aboard about above absent according to across after against ahead ahead of all over along alongside amid amidst among amongst anti around as as of as to aside astraddle astride at away from bar barring because of before behind below beneath beside besides between beyond but by by the time of circa close by close to concerning considering despite down due to during except except for excepting excluding failing following for for all from given in in between in front of in keeping with in place of in spite of in view of including inside instead of into less like minus near near to next to notwithstanding of off on on top of onto opposite other than out out of outside over past pending per pertaining to plus regarding respecting round save saving similar to since than thanks to through throughout thru till to toward towards under underneath unlike until unto up up to upon versus via wanting with within without all another any anybody anyone anything both each each other either everybody everyone everything few he her hers herself him himself his I it its itself many me mine myself neither no_one nobody none nothing one one another other ours ourselves several she some somebody someone something such that theirs them themselves these they this those us we what whatever which whichever who whoever whom whomever whose you yours yourself yourselves 0%, 10%, 50%, 100%, etc. 1, 2, 3, 4, etc. ½, ¼, etc. a bit of a couple of a few a good deal of a good many a great deal of a great many a lack of a little a little bit of a majority of a minority of a number of a plethora of a quantity of all an amount of another any both certain each either enough few fewer heaps of less little loads lots many masses of more most much neither no none numbers of one half, one third, one fourth, one quarter, one fifth, etc. one, two, three, four, etc. part plenty of quantities of several some the lack of the majority of the minority of the number of the plethora of the remainder of the rest of the whole tons of various".Split();
        public static double contentness(String word)
        {
            return word.Length / (function_words.Contains(word) ? 10.0 : 1.0);
        }
        public static double word_relevance(String word, String[] query_words)
        {
            return (query_words.Contains(word) ? contentness(word) : 0);
        }
        public static double sentence_relevance(String sentence, String[] query_words)
        { // returns number > -1.0
            String[] sentence_words = sentence.Split();
            double sum = sentence_words.Sum(x => word_relevance(x, query_words));
            return (sum - 1.0) / sentence.Length;
        }

        public static SortedDictionary<String, double> answer(String text, String query, int num)
        { // returns list of n best sentences
            String[] sentences = text.Split('.');                                             // (ordered by from least to most relevant)
            String[] query_words = query.Split(' ');

            SortedDictionary<String, double> best_so_fars = new SortedDictionary<String, double>();
            double worst_score_in_best = -100.0;
            foreach (String sentence in sentences)
            {
                double score = sentence_relevance(sentence, query_words);
                if (best_so_fars.Count < num)
                {
                    if (!best_so_fars.ContainsKey(sentence))
                    {
                        best_so_fars.Add(sentence, score);
                    }
                    if (score < worst_score_in_best)
                    {
                        worst_score_in_best = score;
                    }
                }
                else if (score > worst_score_in_best)
                {
                    if (!best_so_fars.ContainsKey(sentence))
                    {
                        best_so_fars.Add(sentence, score);
                    }
                    best_so_fars.Remove(best_so_fars.First().Key);
                }
            } return best_so_fars;
        }
    }
}
