using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNCT
{
    public class Phrase : IComparable
    {
        private const double DECAY_RATE = 0.5; /* DECAY_RATE sets importance of intra-text connections
                                                * relative to literal matching to query. The higher the
                                                * DECAY_RATE, the more important is literal matching.
                                                * And the closer to 0 clump the scores, although
                                                * this shouldn't affect ability to be ranked by score.
                                                * 
                                                * DECAY_RATE must range in [0, infinity).
                                                * Probably values in [0, 1.0] are best. We haven't tested
                                                * limit case of 0 --- how good are answers, then?
                                                */
        private const double EQUILIBRIUM_WEIGHT = 0.01; /* See use of EQUILIBRIUM_WEIGHT below.
                                                        * IDK what it does, I just guessed it would
                                                        * smooth out the scores, and perhaps it did.
                                                        * 
                                                        * TODO: what happens when EQUILIBRIUM_WEIGHT==0?
                                                        */

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
            importance -= DECAY_RATE * (importance - EQUILIBRIUM_WEIGHT) * dt;
        }
    }


    public class PhraseGraph
    {
        private SortedDictionary<String, Phrase> phrases;
        private JudgeOfRelevancy JoR;
        public String query;
        public HashSet<String> query_content_words;

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
            phrases = new SortedDictionary<String, Phrase>();
            query = q;
            query_content_words = q_content_words;

            List<Tuple<String, List<String>>> sentence_words = TextCleaner.get_sentence_words(text);
            Phrase last_sentence = null;
            foreach (Tuple<String, List<String>> sentence_and_words in sentence_words)
            {
                String sentence = sentence_and_words.Item1;
                List<String> words = sentence_and_words.Item2;

                Phrase sentence_phrase = ensure_phrase(sentence);
                if (last_sentence != null) // Link neighbor sentences
                {
                    last_sentence.add_recipient(sentence_phrase, 0.5); // 0.5 < 0.6, so weight will drift back:
                    sentence_phrase.add_recipient(last_sentence, 0.6); // so earlier sentences better
                }

                foreach(var word in words) // Link sentences with their words 
                {
                    if(word=="") {continue;}
                    Phrase word_phrase = ensure_phrase(word);
                    word_phrase.add_recipient(sentence_phrase, 10.0 / words.Count()); // weight flows from long sentences
                    sentence_phrase.add_recipient(word_phrase, 1.0);                  // to words, to short sentences
                }

                last_sentence = sentence_phrase;
            }

            foreach(String word in phrases.Keys) // Link synonyms
            {
                if (word.Split().Count() >= 2) { continue; } // don't include sentences
                HashSet<String> syns = JoR.get_synonyms(word);
                foreach(String synonym in syns)
                {
                    Phrase word_phrase = ensure_phrase(word);
                    Phrase syn_phrase = ensure_phrase(synonym);
                    syn_phrase.add_recipient(word_phrase, 1.0*syns.Count());
                    word_phrase.add_recipient(syn_phrase, 1.0*syns.Count());
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
                if(sentence.Split().Count() <= 1) {continue;} // don't include single words.

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
