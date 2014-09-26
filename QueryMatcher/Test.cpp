#include "QueryMatcher.h"
#include "FileReader.h"


#define KNOW_PATH "C:\\Users\\Sam\\Desktop\\SNCT\\fish_knowledgebase.txt"


#define PRINT_RESULTS_OF(query) { \
   cout << "" << endl; \
   list<string> queries = {query}; \
   list<string> results = find_relevant_sentences(knowledgebase, queries); \
   for(list<string>::const_iterator res_ptr = results.begin(); res_ptr != results.end(); ++res_ptr) { \
      cout << (*res_ptr) << endl << endl; \
   } \
}


int main() {
	string knowledgebase = read_string_from(KNOW_PATH);
	/*cout << knowledgebase << endl;*/

   cout << "Results for \"white shark and tuna\":" << endl;
   cout << "***********************************" << endl;
   PRINT_RESULTS_OF("white shark and tuna");

   cout << "Results for \"fish is\" and \"fish are\":" << endl;
   cout << "*************************************" << endl;
   PRINT_RESULTS_OF("fish is", "fish are");
   
   cout << "Tada!" << endl;
   char l;  cin >> l;
}
