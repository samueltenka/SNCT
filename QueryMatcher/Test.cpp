#include "QueryMatcher.h"
#include "FileReader.h"


#define KNOW_PATH "C:\\Users\\Sam\\Desktop\\SNCT\\fish_knowledgebase.txt"


int main() {
	string knowledgebase = read_string_from(KNOW_PATH);
	cout << knowledgebase << endl;

  // FILL IN TEST

	cout << "Tada!" << endl;
	char l;  cin >> l;
}
