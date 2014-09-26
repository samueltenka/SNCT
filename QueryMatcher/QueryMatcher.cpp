#include "QueryMatcher.h"


static string expand_to_surrounding_sentence(string text, int index) {
   /* sentences are minimal intervals (period, period] */
   text = "." + text + ".";
   int left = text.find_last_of('.', index);
   int right = text.find('.', index);
   return text.substr(left+1, right-left);
}

list<string> find_relevant_sentences(string knowledgebase, list<string> queries) {
	// get indices of all matches:
	list<int> match_indices;
   for(list<string>::const_iterator query_ptr = queries.begin(); query_ptr != queries.end(); ++query_ptr) {
      // get indices matching specific query, namely *query_ptr:
	   int index = -1; do {
         index = knowledgebase.find(*query_ptr, index + 1);
         if(index==string::npos) {break;} // ensure that index not at end
         match_indices.push_back(index);
      } while(true);
   }

   list<string> relevant_sentences;
   for(list<int>::const_iterator index_ptr = match_indices.begin(); index_ptr != match_indices.end(); ++index_ptr) {
      relevant_sentences.push_back(expand_to_surrounding_sentence(knowledgebase, *index_ptr));
   } return relevant_sentences;
}

string generate_consensus(list<string> relevant_sentences) {
   return *(relevant_sentences.begin());
   // FILL IN
}
