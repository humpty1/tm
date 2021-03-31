#include<stdio.h>
#include<windows.h>
#include<io.h>
#include<locale.h>
#include<iostream>
#include<vector>
#include <process.h>

#pragma warning (disable :4996)
using namespace std;

#define max_len 1024
#define MSG1 "  no comment" 

struct Vect
{
	char *NM;
	char *KM;
};
int PRINT(FILE *f,vector<Vect*> &lst){

	int i=lst.size();
	for(int j=0;j<i;j++)
		fprintf(f,"\n %s - %s",(lst[j])->NM,(lst[j])->KM+2);
	return i;
}
int FREE(vector<Vect*> &lst){

	Vect *tmp=0;
	int i=lst.size();
	for(int j=0;j<i;j++)
	{
		tmp=(lst[j]);
		if(tmp->NM)
			free(tmp->NM);
		tmp->NM=0;
		if(tmp->KM)
			free(tmp->KM);
		tmp->KM=0;
		free(tmp);
	}
	for(int j=0;j<i;j++)  //delete spisok
		lst.pop_back();
	return 0;
}
int is_number(char *str){
	int i=0;
	while(isspace(str[i])==0 && str[i]!=0)
	{
		if(isdigit(str[i])==0)
			return 0;
		i++;
	}
	return 1;
}


int dFlag = 0; // debug
int hFlag=0; //help
//int vFlag = 0; 

char * mask = "*.tm"; //type of file

Vect *vrem=0;
vector<Vect*> dir;

char *  toW1251(char * str){
	return (str);
}

void main(int argc, char*  argv[]){

	/*_CrtSetDbgFlag(
		_CrtSetDbgFlag(_CRTDBG_REPORT_FLAG)
		| _CRTDBG_CHECK_ALWAYS_DF 
		| _CRTDBG_LEAK_CHECK_DF
		);

	_CrtSetReportMode(_CRT_ASSERT, _CRTDBG_MODE_FILE);
	_CrtSetReportFile(_CRT_ASSERT, _CRTDBG_FILE_STDERR);
	_CrtSetReportMode(_CRT_WARN, _CRTDBG_MODE_FILE);
	_CrtSetReportFile(_CRT_WARN, _CRTDBG_FILE_STDERR);
	_CrtSetReportMode(_CRT_ERROR, _CRTDBG_MODE_FILE);
	_CrtSetReportFile(_CRT_ERROR, _CRTDBG_FILE_STDERR);*/

	struct _finddata_t fiD;
	char * s="";
	int h; 
	int rc = 0;
	int shift = 0;
	int i=0;
	setlocale(LC_ALL, "rus");

	int z2=0;
	int z1=0;

	char str[max_len+1]; //comment script
	char str1[max_len+1]; //result of MT

	for (i=1; i<argc; i++) 
	{
		if (stricmp (argv[i], "-?")==0)	hFlag=1;
		else if (stricmp(argv[i], "-d")==0)	{dFlag=1;   shift=1;    }
		//else if (stricmp(argv[i], "-v")==0)	vFlag=1;
	}
	if(hFlag)
	{
		//HELP
		cout<<"\t This program use a Turing machine"<<endl;
		cout<<"app.exe [-d] [-?] {script(name.tm)} {argument1} [argument2] "<<endl;
		cout<<"\t Where:"<<endl;
		cout<<"script - some file with extention *.tm for MT"<<endl;
		cout<<"argumentX - some number for MT"<<endl;
		cout<<"-d - print debug"<<endl;
		cout<<"-? - help"<<endl;
		cout<<"\t We have this script in case:"<<endl;
	}

	for (h=_findfirst(mask,&fiD); h>=0 && rc==0; rc = _findnext(h,&fiD)) //zapolnenie vectora imenem scripta i ego comenta
	{
		/*if (fiD.attrib & _A_SUBDIR)
			s="it is directory";
		else 
			s="it is file";*/
		vrem=(Vect *)malloc(sizeof(Vect));
		vrem->NM=(char *)malloc(strlen(fiD.name)+1);
		strcpy(vrem->NM,fiD.name); //take name script
		dir.push_back(vrem);

		char* fl=toW1251(fiD.name);
		FILE* f=fopen (fl,"r");
		if(f)
		{
			if(fgets(str,max_len+1,f))
			{
				int dl=strlen(str);
				if(dl>2 && str[0]=='/' && str[1]=='/')
				{
					if(dl)
						str[dl-1]=0;
					vrem->KM=(char *)malloc(dl+1); 
					strcpy(vrem->KM,str); //take first comment
				}
				else
				{
					vrem->KM=(char *)malloc(strlen(MSG1)+1);
					strcpy(vrem->KM,MSG1); //if we haven't comment
				}
			}
			fclose(f);
		}
		else
			perror(toW1251(fiD.name));
	}
	_findclose (h);

	if(hFlag)
		PRINT(stderr,dir);

	if(argv[2+shift])
	{
		if(is_number(argv[2+shift]))
			z1=atoi(argv[2+shift]);
		else
		{
			cout<<"\t Input first argument isn't corect: '"<<argv[2+shift]<<"'"<<endl;
			exit(1);
		}
	}
	else
	{
		cout<<"\t Please input argument!"<<endl;
		exit(1);
	}
	if(argv[3+shift])
	{
		if(is_number(argv[3+shift]))
			z2=atoi(argv[3+shift]);
		else
		{
			cout<<"\t Input second argument isn't corect: '"<<argv[3+shift]<<"'"<<endl;
			exit(1);
		}
	}
	/*else
	{
		cout<<"\t Please input second argument!"<<endl;
		exit(1);
	}*/

	char arg[100]={0}; //register for MT
	char *_argv[11]={0}; //komand stroka for MT
	int ln1=0;
	int ln2=0;
	char *tm=0;
	
	//zapolnenie registra for MT
	arg[0]='L';
	for(ln1=1;ln1<=z1;ln1++)
		arg[ln1]='|';

	if(argv[3+shift]) //if two arguments
	{
		arg[ln1]='#';
		for(ln2=ln1+1; ln2<=z2+z1+1; ln2++)
			arg[ln2]='|';
		arg[ln2]='L';
	}
	else
		arg[ln1]='L';

	if(dFlag)
	{
		//print a register for MT
		cout<<"register is : '";
		for(int i=0;i<=ln2;i++)
			cout<<arg[i]<<" ";
		cout<<"'"<<endl;
	}

	_argv[0]="tm1.exe";
	_argv[1]="-b";
	_argv[2]="L"; //empty-sybol for MT
	_argv[3]="-r";
	_argv[4]=arg; //stroka with register for MT
	_argv[5]="-f";
	_argv[6]=argv[1+shift]; //script
	_argv[7]="-o";
	_argv[8]="1.txt"; //output file MT
	_argv[9]="-v";
	_argv[10]=0;
	rc = _spawnv(_P_WAIT,"tm1.exe",_argv);
	if (dFlag) {
		cout<<"**return code is "<<rc;
	
	}
	if (rc ==0 ) {
   	FILE* t=fopen ("1.txt","r");
   		if(t)
   		{
   			if(fgets(str1,max_len+1,t))
   			{
   				tm=strtok(str1,"L"); //perevod resultata MT v cifru
   				if(tm)
   				{
   					if(dFlag)
   						cout<<"\n**result is: '"<<tm<<"'"<<endl;
   					cout<<strlen(tm)<<endl; 
   				}
   				else
   					cout<<'0'<<endl;
   			}
   			fclose(t);
   		}
   		else
   			perror("1.txt");
	 }
	 else 
		cout<<"\nsomething is wrong. no result";

		FREE(dir);
}
