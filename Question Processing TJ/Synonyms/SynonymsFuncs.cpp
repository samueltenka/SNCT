#include "Synonyms.h"

void questionSplit(const string &question,
	vector<string> &questionSplited, char del) {
	string tempStr;
	for (int i = 0; i < question.length(); i++) {
		if (question[i] == ' ' || question[i] == del) {
			if (!tempStr.empty()) {
				questionSplited.push_back(tempStr);
				tempStr.clear();
			}
		}
		else {
			tempStr += question[i];
		}
	}
	if (!tempStr.empty()) {
		questionSplited.push_back(tempStr);
		tempStr.clear();
	}
}

void combine(vector<string> &combinations,
	const vector<string> &questionSplited) {
	int i = 0;
	int j = 0;
	string tempStr;
	while (j <= questionSplited.size()) {
		int numComb = 3;
		int numNexts = questionSplited.size() - j - 1;
		if (numNexts < 3) {
			numComb = numNexts;
		}
		for (int k = 0; k <= numComb; k++) {
			for (int l = 0; l <= k; l++) {
				if (l != 0) {
					tempStr += ' ';
				}
				tempStr
					+= questionSplited[j + l];
			}
			combinations.push_back(tempStr);
			tempStr.clear();
		}
		j++;
	}
}

bool stringCompare(string a, string b) {
	return a.compare(b) < 0;
}

void openNewFile(const vector<string> &questionSplited,
	ifstream &dic, string &dicName, int i) {
	dic.close();
	dicName = "dic-";
	dicName.push_back(questionSplited[i][0]);
	dicName.push_back('-');
	dicName.push_back(questionSplited[i][1]);
	dicName.append(".txt");
	dic.open(dicName.c_str());
}

void typeInit(const vector<string> &questionSplited,
	vector<Word> &synonyms, ifstream &dic,
	string &dicInput, int i, int &numTypes) {
	Type wordType;
	if (dicInput != "--------word type--------") {
		getline(dic, dicInput);
	}
	getline(dic, wordType.text);
	synonyms[i].type.push_back(wordType);
	numTypes++;

	getline(dic, dicInput);
	getline(dic, dicInput);
	int numSyn = 0;
	while (dic.good() && dicInput != "-----------word----------" &&
		dicInput != "--------word type--------") {
		synonyms[i].type[numTypes - 1].synonyms.push_back(dicInput);
		numSyn++;
		getline(dic, dicInput);
	}
	if (numSyn == 0) {
		synonyms[i].type[numTypes - 1].synonyms.push_back("(none)");
	}
}

void wordInit(const vector<string> &questionSplited,
	vector<Word> &synonyms, ifstream &dic, int i) {
	int numTypes = 0;
	string dicInput;
	getline(dic, dicInput);
	while (dic.good() && dicInput != "-----------word----------") {
		typeInit(questionSplited, synonyms, dic, dicInput, i, numTypes);
		if (!dic.good()) {
			dic.close();
			continue;
		}
	}
	if (numTypes == 0) {
		synonyms[i].type.push_back({ "(none)" });
	}
	if (!dic.good()) {
		dic.close();
	}
}

void synonymsInit(const vector<string> &questionSplited,
	vector<Word> &synonyms) {
	ifstream dic;
	string dicName = "dic-0-0.txt";
	Word init;
	for (int i = 0; i < questionSplited.size(); i++) {
		string dicInput;
		init.text = questionSplited[i];
		synonyms.push_back(init);

		if (dicName[4] != questionSplited[i][0] ||
			dicName[6] != questionSplited[i][1]) {
			openNewFile(questionSplited, dic, dicName, i);
		}

		while (dic.good()) {
			getline(dic, dicInput);
			if (dicInput == "-----------word----------") {
				getline(dic, dicInput);
				if (dicInput == questionSplited[i]) {
					wordInit(questionSplited, synonyms, dic, i);
					break;
				}
			}
		}
		if (!dic.good()) {
			synonyms[i].type.push_back({ "(not found in dictionary)" });
			dic.close();
		}
	}
	dic.close();
}

template <typename T>
void print(const T &vector) {
	for (auto i = vector.begin(); i < vector.end(); i++) {
		cout << *i << endl;
	}
	cout << endl;
}

void printSyns(const vector<Word> &synonyms) {
	for (auto i = synonyms.begin(); i < synonyms.end(); i++) {
		cout << i->text << "\n";
		for (auto j = i->type.begin(); j < i->type.end(); j++) {
			cout << "\t" << j->text << "\n";
			if (j->text != "(none)") {
				for (auto k = j->synonyms.begin(); k < j->synonyms.end(); k++) {
					cout << "\t\t" << *k << "\n";
				}
			}
		}
	}
}