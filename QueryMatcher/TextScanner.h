#ifndef TEXT_SCANNER
#define TEXT_SCANNER



#include <list>
#include <string>
using namespace std;


/**********************************************************************
// LOCATE_MENTIONS_OF:
//    returns start-indices of matches.
//    we first define function for query singular, then queries plural.
**********************************************************************/
static list<int> locate_mentions_of(string knowledgebase, string query) { // query singular
   list<int> match_indices;
   int index = -1; do {
      index = knowledgebase.find(query, index + 1);
      if(index==string::npos) {break;} // ensure that index not at end
      match_indices.push_back(index);
   } while(true);
   return match_indices;
}
list<int> locate_mentions_of(string knowledgebase, list<string> queries) { // queries plural
   list<int> match_indices;
   for(list<string>::const_iterator query_ptr = queries.begin(); query_ptr != queries.end(); ++query_ptr) {
      list<int> query_results = locate_mentions_of(knowledgebase, *query_ptr);
      match_indices.splice(match_indices.end(), query_results); // append to end
   } return match_indices;
}

/*******************************************************************
// EXPAND_TO_SURROUNDING_SENTENCES:
//    e.g. expands index 4 in "a. be. c." to " be."
//                             012345678
//    first define func. for "sentence" sng., then "sentences" pl.
*******************************************************************/
static string expand_to_surrounding_sentence(string text, int index) {
   /* sentences are minimal intervals (period, period] */
   text = "." + text + "."; index += 1;
   int left = text.find_last_of('.', index);
   int right = text.find('.', index);
   return text.substr(left+1, right-left);
}
list<string> expand_to_surrounding_sentences(string text, list<int> indices) {
   list<string> sentences;
   for(list<int>::const_iterator index_ptr = indices.begin(); index_ptr != indices.end(); ++index_ptr) {
      sentences.push_back(expand_to_surrounding_sentence(text, *index_ptr));
   } return sentences;
}


#endif // TEXT_SCANNER
