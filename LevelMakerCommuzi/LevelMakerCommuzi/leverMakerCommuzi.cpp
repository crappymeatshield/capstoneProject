#include <iostream>
#include <fstream>
#include <string>
#include <vector>
using namespace std;

void main()
{
	fstream fout;
	fstream fin;
	string level;
	string fileName;
	int boxnum;



	cout << "Enter the level Name: ";
	cin >> level;
	fileName = level + " level";
	fileName += ".xml";
	fout.open(fileName.c_str(),ios::out) ;
	fileName=level+" level.txt";
	fin.open(fileName.c_str(),ios::in);

	fout << "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" << endl;
	fout << "<XnaContent>" << endl;
	fout << "<Asset Type=\"System.Collections.Generic.List[XmlContentSampleShared.Sprite]\">" << endl;

	//cout<< "enter numerical map: \n";
	for(int i=1; i<=3776; i++)//3776
	{
		fin>>boxnum;
		if(boxnum>=8)
		{
			fout<<"<Item>\n";
			fout<<"<Floorsquare>"<<i<<"</Floorsquare>\n";
			fout<<"<Boxnum>8</Boxnum>\n";
			fout<<"</Item>\n";
			boxnum-=8;
		}
		if(boxnum>=4)
		{
			fout<<"<Item>\n";
			fout<<"<Floorsquare>"<<i<<"</Floorsquare>\n";
			fout<<"<Boxnum>4</Boxnum>\n";
			fout<<"</Item>\n";
			boxnum-=4;
		}
		if(boxnum>=2)
		{
			fout<<"<Item>\n";
			fout<<"<Floorsquare>"<<i<<"</Floorsquare>\n";
			fout<<"<Boxnum>2</Boxnum>\n";
			fout<<"</Item>\n";
			boxnum-=2;
		}
		if(boxnum>=1)
		{
			fout<<"<Item>\n";
			fout<<"<Floorsquare>"<<i<<"</Floorsquare>\n";
			fout<<"<Boxnum>1</Boxnum>\n";
			fout<<"</Item>\n";
			boxnum-=1;
		}
	}
	fout<<"</Asset>\n";
	fout<<"</XnaContent>";
	fin.close();
	fout.close();
}