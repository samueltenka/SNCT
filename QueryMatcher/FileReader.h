#ifndef FILE_READER
#define FILE_READER



#include <string>
#include <fstream>
#include <sstream>
#include <iostream>

using namespace std;

string read_string_from(string filepath) {
	ifstream knowledgebase(filepath);
	if(knowledgebase.is_open()) {
		stringstream buffer;
		buffer << knowledgebase.rdbuf();
		return buffer.str();
	}
	else {
		cout << "OUCH! couldn't find file " << filepath << "!" << endl;
	}
	knowledgebase.close();
}


#endif // FILE_READER
