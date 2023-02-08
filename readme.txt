Overview: 
DataGenerator - Program is generating text file with sample data taken from file dataset.cs. Data is in format: ,,Number. Text"

Input - number of Gb to generate and optional output file name eg.: 10 TestData.txt
Output- TestData.txt 

Notes:
Generation of 1GB of data on my 10 years old pc takes around 80-90s.
Program was tested only on x64 bits

DataSorter - Program is sorting file generated in utility mentioned above. Sorting is done using external sort aproach with tin sort(generic) and merge sort used. Sorting criteria are as folows: first part of string then number. 

Input - input path, output path eg.: c:\data\input.txt c:\data\output.txt
Output- output.txt

Notes:
Sorting 1GB of data takes on my pc around 12 min


