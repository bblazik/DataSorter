Overview: Program is generating text file with sample data taken from file dataset.cs. Data is in format: ,,Number. Text"

Input - number of Gb to generate and optional output file name eg.: 10 TestData.txt
Output- TestData.txt 

Notes:
Generation of 100gb of data on my 10 years old pc takes around 5h.
One of the warning with generation of file 100GB+ can be overflow of long variable. Program can be changed to not store total number of records with further optimalization.
Program was tested only on x64 bits
