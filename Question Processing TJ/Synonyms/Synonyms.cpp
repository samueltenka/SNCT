#include "Synonyms.h"

int main() {
	string question;
	getline(cin, question);

	vector<string> questionSplited;
	questionSplit(question, questionSplited, '?');
	sort(questionSplited.begin(), questionSplited.end(), stringCompare);

	vector<Word> synonyms;
	synonymsInit(questionSplited, synonyms);

	printSyns(synonyms);
	return 0;
}