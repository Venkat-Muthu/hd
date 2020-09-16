# hd

1. Project setup Pre-requisite :<br/>
	1. Net 5 Preview<br/>
	2. C# 9.0<br/>
	3. Github installed (optional)<br/>
2. Clone/Download the Source Code to local machine :<br/>
3. Open a cmd window, navigate to the project folder<br/>
4. Run "dotnet run .\UrlRanking.csproj"<br/>
<br/>
<br/>
Technical implementation details : <br/>
1. The project has two major modules. <br/>
	a. LogSimulator will create a log file called : file01.txt. The will simulate simplified W3C log file with URL : /images/picture"X".jpg, where "X" is mod of 10.<br/>
	b. LogReader will read the file : file01.txt simultaneously and updates the value in the data store.<br/>
	c. A timer runs at the interval of 5 seconds display the top 5 most visited URLs in live.<br/>
	d. Press any key to termnate the application.<br/>
	e. Ranking can be compared at any time with the file : file01.txt<br/>

Output
![Output Screenshot](Screenshots/output.jpg?raw=true "Output")