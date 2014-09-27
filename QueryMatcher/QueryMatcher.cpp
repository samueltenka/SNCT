#include "QueryMatcher.h"
#include "TextScanner.h"""


list<string> find_relevant_sentences(string knowledgebase, list<string> queries) {
   list<int> match_indices = locate_mentions_of(knowledgebase, queries);
   return expand_to_surrounding_sentences(knowledgebase, match_indices);
}

string generate_consensus(list<string> relevant_sentences) {
   return *(relevant_sentences.begin());
   // FILL IN
}


