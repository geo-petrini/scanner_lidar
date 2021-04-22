CONTENTS OF THIS FILE
---------------------

 * Introduction
 * Pre-Requirements
 * Starting the Application

INTRODUCTION
------------

Group Members:
	- Matteo Lupica <matteo.lupica@samtrevano.ch> 
	- Daniele Cereghetti <daniele.cereghetti@samtrevano.ch> 
	- Veljko Markovic <veljko.markovic@samtrevano.ch> 
	- Isaac Gragasin <isaac.gragasin@samtrevano.ch> 

The Scanner_Lidar software allows a server, after processing the data coming from
a scanner with arduino, to distribute them to all of the clients
that will connect to the server, and each one of them will be able to visualize them.
This application has been developed in C# and the source code
is available on https://github.com/geo-petrini/scanner_lidar

PRE-REQUIREMENTS
------------

.NET Core 3.1O must be installed on this machine before application startup. => https://dotnet.microsoft.com/download
Arduino must already be attached on this machine before application startup.
Change values in Release/Server_Lidar.dll.config according to what you need

STARTING THE APPLICATION
----------------

If all of the pre-requirements are satisfied, simply 
run the Start.bat file to start the application.

After launching the Start.bat has been start, allow the firewall request.