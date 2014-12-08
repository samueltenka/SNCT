using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SNCT
{
    class JudgeOfRelevancy
    {
        async private static Task<String> get_text(String filename)
        {
            var folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var file = await folder.GetFileAsync(filename);
            var contents = await Windows.Storage.FileIO.ReadTextAsync(file);
            return contents;
        }

        private SortedDictionary<String, HashSet<String>> thesaurus;
        async public Task JudgeOfRelevancy()
        {
            // initialize thesaurus
            String thes_text = await get_text("thesaurus\thesaurus.txt");

            bool new_word = false;
            String word = "";
            foreach(String line in thes_text.Split('\n'))
            {
                if(new_word) {
                    word = line;
                    thesaurus.Add(word, new HashSet<String>());
                    thesaurus[word].Add(word);
                    new_word = false;
                } else if(line=="-----------word----------") {
                    new_word = true;
                } else if(line[0] != '-') {
                    thesaurus[word].Add(line);
                }
            }
        }
        
        private HashSet<String> get_synonyms(String word)
        {
            if (thesaurus.ContainsKey(word)) { return thesaurus[word]; }
            else { return new HashSet<String> { word }; }
        }

        private static String[] time_words = ("nanosecond millisecond second minute hour day week month season year decade century millenium" +
                                              "before during after past present future previous current next" +
                                              "then now early late first last" +
                                              "sunday monday tuesday wednesday thursday friday saturday" +
                                              "january february march april may june july august september october november december" +
                                              "winter spring summer fall" +
                                              "18th 19th 20th 21st" +
                                              "1776 1975 1945 2011 2012 2013 2014" +
                                              "28th 29th 30th 31st" +
                                              "revolutionary independence civil world war history time").Split();
        private static String[] place_words = ("chair table cubby desk shelf closet drawer" +
                                               "room building neighborhood village town zone city district county peninsula country continent earth planet system" + 
                                               "on under in out inside outside right left up down above below here there" +
                                              "east west north south" +
                                              "address room house street avenue lane road drive bridge interstate highway way" +                                              
                                              "Albuquerque Ann Arbor Detroit Redmond NYC LA angeles" +
                                              "MI IN OH CA NY WA michigan indiana ohio california york washington" +
                                              "USA united states canada mexico china brazil japan france britain germany india italy spain" +
                                              "america australia asia africa europe antarctica" +
                                              "geology geography place space").ToLower().Split();
        private static String[] person_words = ("he she they his her their his hers theirs him her them").Split();
        private static String[] identity_words = ("that this is definition").Split();
        private static String[] specifying_words = ("of").Split();
        private static String[] explanation_words = ("because since for reason order to why").Split();
        private static String[] mechanism_words = ("via using with how").Split();
        private static String[] truth_words = ("indeed truly does do did").Split();


        private String[] get_category_words_of(String query)
        {
            String first = query.Split()[0]; // TODO: how about "For what reason does...?"?
            String[] category_words = first=="when" ? time_words :
                                      first=="where" ? place_words :
                                      first=="who" ? person_words :
                                      first=="what" ? identity_words :
                                      first=="which" ? specifying_words :
                first=="wherefore" || first=="why" ? explanation_words :
                                      first=="how" ? mechanism_words : 
                                    /*auxilliary*/ truth_words; // e.g. "Is Santa Claus real?"
            return category_words;
        }

        private double get_word_similarity(String word1, String word2)
        { // TODO: make fuzzy!
            HashSet<String> words1 = get_synonyms(word1);
            HashSet<String> words2 = get_synonyms(word2);
            return words1.Intersect(words2).Count() / (words1.Count() + words2.Count());
        }
        public double get_direct_relevance(String text, String query, HashSet<String> query_content_words)
        {
            String[] query_category_words = get_category_words_of(query);
            String[] text_words = text.Split();
            double score = 0.0;
            foreach(String word in text_words)
            {
                double category_score = 0.0, content_score = 0.0;
                foreach (String category_word in query_category_words) { category_score += get_word_similarity(word, category_word); }
                foreach (String content_word in query_content_words) { content_score += get_word_similarity(word, content_word); }
                score += category_score / Math.Pow(query_category_words.Count(), 0.5) + // 0.5 because length doesn't really matter
                         content_score / Math.Pow(query_content_words.Count(), 1.0);    // 1.0 as standard scaling power
            }
            return score / Math.Pow(text_words.Count(), 1.5);                           // 1.5 to penalize long sentences
        }
    }
}
