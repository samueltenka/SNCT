#ifndef _SYNONYMS_H
#define _SYNONYMS_H

#include <fstream>
#include <iostream>
#include <string>
#include <vector>
#include <algorithm>
using namespace std;

struct Type {
	string text;
	//Noun			:	n;
	//Pronoun		:	o;
	//Adjective		:	j;
	//Verb			:	v;
	//Adverb		:	d;
	//Preposition	:	p;
	//Conjunction	:	c;
	//Interjection	:	i;
	vector<string> synonyms;
};

struct Word{
	string text;
	vector<Type> type;
};

void questionSplit(const string &question,
	vector<string> &questionSplited, char del);

void combine(vector<string> &combinations,
	const vector<string> &questionSplited);

bool stringCompare(string a, string b);

void openNewFile(const vector<string> &questionSplited,
	ifstream &dic, string &dicName);

void typeInit(const vector<string> &questionSplited,
	vector<Word> &synonyms, ifstream &dic,
	string &dicInput, int i, int &numTypes);

void wordInit(const vector<string> &questionSplited,
	vector<Word> &synonyms, ifstream &dic, int i);

void synonymsInit(const vector<string> &questionSplited,
	vector<Word> &synonyms);

template <typename T>
void print(const T &vector);

void printSyns(const vector<Word> &synonyms);

#endif