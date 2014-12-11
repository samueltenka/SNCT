using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SNCT
{
    public class Phrase : IComparable
    {
        /* DECAY_RATE sets importance of intra-text connections
         * relative to literal matching to query. The higher the
         * DECAY_RATE, the more important is literal matching.
         * And the closer to 0 clump the scores, although
         * this shouldn't affect ability to be ranked by score.
         * 
         * DECAY_RATE must range in [0, infinity).
         * Probably values in [0, 1.0] are best. We haven't tested
         * limit case of 0 --- how good are answers, then?
         */
        private const double DECAY_RATE = 0.1;

        /* See use of EQUILIBRIUM_WEIGHT below. 
         * IDK what it does, I just guessed it would 
         * smooth out the scores, and perhaps it did. 
         *  
         * TODO: what happens when EQUILIBRIUM_WEIGHT==0? 
         */
        private const double EQUILIBRIUM_WEIGHT = 0.1;

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

        public void add_recipient(Phrase recipient, double weight) { outs[recipient] = weight; }
        public double get_importance() { return importance; }

        public void step(double dt)
        {
            importance += influx * dt;
            foreach (var pair in outs)
            {
                double flux = importance * pair.Value;
                pair.Key.importance += flux * dt;
                importance -= flux * dt;
            }
            importance -= DECAY_RATE * (importance - 0.1) * dt;
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
            foreach (char c in "?!.;:,\"\t\n")
            {
                text = text.Replace(c.ToString(), "");
            }
            text = text.Replace("  ", "");
            return text.Split();
        }

        public Phrase ensure_phrase(String text)
        {
            if (!phrases.ContainsKey(text))
            {
                double rel = JoR.get_direct_relevance(text, query, query_content_words);
                phrases.Add(text, new Phrase(text, rel));
            }
            return phrases[text];
        }

        async public static Task<PhraseGraph> build_PG(String text, String q, HashSet<String> q_content_words)
        {
            JudgeOfRelevancy JoR_ = await JudgeOfRelevancy.build_JoR();
            return new PhraseGraph(JoR_, text, q, q_content_words);
        }

        public PhraseGraph(JudgeOfRelevancy JoR_, String text, String q, HashSet<String> q_content_words)
        {
            JoR = JoR_;
            phrases = new SortedDictionary<String, Phrase>();
            query = q;
            query_content_words = q_content_words;

            String[] sentences = get_sentences(text.ToLower());
            Phrase last_sentence = null;
            foreach (var sentence in sentences)
            {
                Phrase sentence_phrase = ensure_phrase(sentence);
                // link neighboring sentences
                if (last_sentence != null)
                {
                    // 0.5 < 0.6 so weight will drift back
                    // earlier sentences are better
                    last_sentence.add_recipient(sentence_phrase, 0.5);
                    sentence_phrase.add_recipient(last_sentence, 0.6);
                }
                String[] words = get_words(sentence);
                // link sentences with their words
                foreach (var word in words)
                {
                    if (word == "") { continue; }
                    Phrase word_phrase = ensure_phrase(word);
                    // weight flows from long sentences to words, to short sentences
                    word_phrase.add_recipient(sentence_phrase, 1.0);
                    sentence_phrase.add_recipient(word_phrase, 1.0);
                }
                last_sentence = sentence_phrase;
            }

            // link synonyms
            //foreach (var word in phrases.Keys)
            //{
            //    // don't include sentences
            //    if (word.Split().Count() >= 2) { continue; }
            //    HashSet<String> syns = JoR.get_synonyms(word);
            //    foreach (String synonym in syns)
            //    {
            //        // TODO: fix this :D can't modify the phrases collection while we're enumerating phrases.Keys!
            //        Phrase word_phrase = ensure_phrase(word);
            //        Phrase syn_phrase = ensure_phrase(synonym);
            //        syn_phrase.add_recipient(word_phrase, 1.0 * syns.Count());
            //        word_phrase.add_recipient(syn_phrase, 1.0 * syns.Count());
            //    }
            //}
        }

        public void step(double dt) // todo:
        {
            foreach (var pair in phrases)
            {
                pair.Value.step(dt);
            }
        }

        public SortedDictionary<String, double> get_top(int n)
        {
            SortedDictionary<String, double> best_so_fars = new SortedDictionary<String, double>();
            double worst_score_in_best = -100.0; // assume dt small enough so nobody negative
            foreach (var pair in phrases)
            {
                double score = pair.Value.get_importance();
                String sentence = pair.Key;
                // don't include single words
                if (sentence.Split().Count() <= 1) { continue; }

                if (best_so_fars.Count() < n)
                {
                    if (!best_so_fars.ContainsKey(sentence)) { best_so_fars.Add(sentence, score); }
                    if (score < worst_score_in_best) { worst_score_in_best = score; }
                }
                else if (score > worst_score_in_best)
                {
                    if (!best_so_fars.ContainsKey(sentence)) { best_so_fars.Add(sentence, score); }
                    best_so_fars.Remove(best_so_fars.First().Key);
                }
            }
            return best_so_fars;
        }
    }
}
