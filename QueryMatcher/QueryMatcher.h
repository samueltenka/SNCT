#ifndef QUERY_MATCHER
#define QUERY_MATCHER



#include <string>
#include <list>
using namespace std;


/*****************************************************************************
// FIND_RELEVANT_SENTENCES:
//    searches for sentences containing an element of ``queries'' in 
//    large ``knowledgebase''.
*****************************************************************************/
list<string> find_relevant_sentences(string knowledgebase, list<string> queries);

/************************************************************
// GENERATE_CONSENSUS:
//    based on (hopefully related) ``relevant_sentences'',
//    generates coherent summary.
************************************************************/
string generate_consensus(list<string> relevant_sentences);


#endif QUERY_MATCHER
