using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNCT
{
    public class Phrase
    {
        public String value;
        private double importance;
        //private List<Phrase> uppers; // phrases containing Phrase
        //private List<Phrase> lowers; // phrases this Phrase contains
        private SortedDictionary<Phrase, double> outs;

        public Phrase(String v)
        {
            value = v;
            importance = 1.0;
        }

        public void add_recipient(Phrase recipient, double weight)
        {
            outs[recipient] = weight;
        }

        public void step(double influx, double dt)
        {
            importance += influx * dt;
            foreach (var pair in outs)
            {
                double flux = importance * pair.Value;
                pair.Key.importance += flux * dt;
                importance -= flux * dt;
            }
            importance -= importance * dt;
        }
    }


    public class PhraseGraph
    {
        private SortedDictionary<String, Phrase> phrases;

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
                phrases.Add(text, new Phrase(text));
            } return phrases[text];
        }

        public PhraseGraph(String text)
        {
            String[] sentences = get_sentences(text.ToLower());
            foreach (var sentence in sentences)
            {
                Phrase sentence_phrase = ensure_phrase(sentence);
                String[] words = get_words(sentence);
                foreach (var word in words)
                {
                    Phrase word_phrase = ensure_phrase(word);
                    word_phrase.add_recipient(sentence_phrase, 1.0);
                    sentence_phrase.add_recipient(word_phrase, 1.0);
                }
            }
        }

        public void step(String query, double dt)
        {
            foreach (var pair in phrases)
            {
                double influx = direct_relevance(pair.Key, query);
                pair.Value.step(influx, dt);
            }
        }
    }
}
