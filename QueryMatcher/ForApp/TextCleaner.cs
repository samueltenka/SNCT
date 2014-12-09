using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace SNCT
{
    class TextCleaner
    {
        public static List<Tuple<String, List<String>>> get_sentence_words(String text)
        {
            List<Tuple<String, List<String>>> sentence_words = new List<Tuple<String, List<String>>>();

            text = /*simplify_sentences(*/remove_citations(text)/*)*/;
            List<String> sentences = get_sentences(text);
            foreach(String sentence in sentences)
            {
                String clean_sentence = normalize_spacing(remove_punctuation(to_lowercase(sentence)));
                List<String> clean_words = new List<String>(get_words(clean_sentence));
                sentence_words.Add(new Tuple<String, List<String>> (clean_sentence, clean_words));
            }
            return sentence_words;
        }
        private static String remove_citations(String text)
        {
            string citation_pattern = @"\[.*\]";
            Regex rgx = new Regex(citation_pattern, RegexOptions.IgnoreCase);
            return rgx.Replace(text, "");
        }

        private static List<String> get_sentences(String text)
        {
            string sentence_boundary_pattern = @"\s\S\S+\.\s+[A-Z]"; // "\s\S\S+" so don't break at "John F. Kennedy"
            Regex rgx = new Regex(sentence_boundary_pattern, RegexOptions.IgnoreCase);
            MatchCollection matches = rgx.Matches(text);
            if (matches.Count > 0)
            {
                List<String> sentences = new List<String>(); // expect of length <= matches.Count + 1
                for (int i = -1; i < matches.Count; ++i)
                {
                    int start = (i < 0 ? -1 : matches[i].Index);
                    int end = (i+1 < matches.Count ? matches[i+1].Index : text.Length);
                    sentences.Add(text.Substring(start+1, end-start-1) + ".");
                }
                return sentences;
            }
            return new List<String>();
        }

        // TODO: keep track of indices of starts of sentences so can go back to upper
        private static String to_lowercase(String text) {return text.ToLower();}
        private static String simplify_sentences(String text)
        {
            foreach(char c in "?!.;:()") { text = text.Replace(c.ToString(), "."); }
            return text;
        }
        private static String remove_punctuation(String text)
        {
            foreach (char c in "?!.;:,") {text = text.Replace(c.ToString(), "");}
            return text;
        }
        private static String normalize_spacing(String text) 
        {
            foreach (char c in "\t\n") { text = text.Replace(c.ToString(), " "); }
            return text.Replace("  ", " ");
        }
        private static String[] get_words(String text) {return text.Split();}
    }
}
