using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

#define DECAY_RATE 0.1

namespace SNCT
{
    public class Phrase : IComparable
    {
        public String value;
        private double influx;
        private double importance;
        //private List<Phrase> uppers; // phrases containing Phrase
        //private List<Phrase> lowers; // phrases this Phrase contains
        private SortedDictionary<Phrase, double> outs;

        public int CompareTo(object obj)
        {
            if (obj == null) { return 1; }
            Phrase P = obj as Phrase;
            if (P != null) { return value.CompareTo(P.value); }
            else { throw new ArgumentException("Object is not a Phrase"); }
        }

        public Phrase(String val, double inf)
        {
            value = val;
            influx = inf;
            importance = 1.0;
            outs = new SortedDictionary<Phrase, double>();
        }

        public void add_recipient(Phrase recipient, double weight) {outs[recipient] = weight;}
        public double get_importance() {return importance;}

        public void step(double dt)
        {
            importance += influx * dt;
            foreach (var pair in outs)
            {
                double flux = importance * pair.Value;
                pair.Key.importance += flux * dt;
                importance -= flux * dt;
            }
            importance -= DECAY_RATE * (importance-0.1) * dt;
        }
    }


    public class PhraseGraph
    {
        private SortedDictionary<String, Phrase> phrases;
        private JudgeOfRelevancy JoR;
        public String query;
        public HashSet<String> query_content_words;

        public static String[] get_sentences(String text)
        {
            string pattern = @"\.\s+[A-Z]";
            Regex rgx = new Regex(pattern, RegexOptions.IgnoreCase);
            MatchCollection matches = rgx.Matches(text);
            if (matches.Count > 0)
            {
                String[] sentences = new String[matches.Count + 1];
                for (int i = -1; i < matches.Count; ++i)
                {
                    int start = (i < 0 ? -1 : matches[i].Index);
                    int end = (i + 1 < matches.Count ? matches[i + 1].Index : text.Length);
                    sentences[i + 1] = text.Substring(start + 1, end - start - 1) + ".";
                }
                return sentences;
            }
            return new String[1];
        }
        public static String[] get_words(String text)
        {
            return text.Split(' ');
        }

        public Phrase ensure_phrase(String text)
        {
            if (!phrases.ContainsKey(text))
            {
                double rel = JoR.get_direct_relevance(text, query, query_content_words);
                phrases.Add(text, new Phrase(text, rel));
            } return phrases[text];
        }

        async public static Task<PhraseGraph> build_PG(String text, String q, HashSet<String> q_content_words)
        {
            JudgeOfRelevancy JoR_ = await JudgeOfRelevancy.build_JoR();
            return new PhraseGraph(JoR_, text, q, q_content_words);
        }
        public PhraseGraph(JudgeOfRelevancy JoR_, String text, String q, HashSet<String> q_content_words)
        {
            JoR = JoR_;
            phrases = new SortedDictionary<string, Phrase>();
            query = q;
            query_content_words = q_content_words;

            String[] sentences = get_sentences(text.ToLower());
            foreach (var sentence in sentences)
            {
                Phrase sentence_phrase = ensure_phrase(sentence);
                String[] words = get_words(sentence);
                foreach(var word in words)
                {
                    if(word=="") {continue;}
                    Phrase word_phrase = ensure_phrase(word);
                    word_phrase.add_recipient(sentence_phrase, 1.0);
                    sentence_phrase.add_recipient(word_phrase, 1.0);
                }
            }
        }

        public void step(double dt) // todo:
        {
            foreach(var pair in phrases)
            {
                pair.Value.step(dt);
            }
        }

        public SortedDictionary<String, double> get_top(int n)
        {
            SortedDictionary<String, double> best_so_fars = new SortedDictionary<String, double>();
            double worst_score_in_best = -100.0; // assume dt small enough so nobody negative
            foreach(var pair in phrases)
            {
                double score = pair.Value.get_importance();
                String sentence = pair.Key;
                if(sentence.Split().Count() <= 3) {continue;} // don't include single words.

                if(best_so_fars.Count() < n)
                {
                    if(!best_so_fars.ContainsKey(sentence)) {best_so_fars.Add(sentence, score);}
                    if(score < worst_score_in_best) {worst_score_in_best = score;}
                }
                else if(score > worst_score_in_best)
                {
                    if(!best_so_fars.ContainsKey(sentence)) {best_so_fars.Add(sentence, score);}
                    best_so_fars.Remove(best_so_fars.First().Key);
                }
            } return best_so_fars;
        }
    }
}
