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

The Scanner_Lidar software processes the data coming from
a scanner with Arduino and allows a server to distribute them to all of the clients
that will connect to it, offering a visual representation to each one of them.
This application has been developed in C# and the source code
is available on https://github.com/geo-petrini/scanner_lidar

PRE-REQUIREMENTS
------------

.NET Core 3.1O must be installed on this machine before application startup. => https://dotnet.microsoft.com/download
The Arduino must already be attached on this machine before application startup.
Change values in Release/Server_Lidar.dll.config to fit the Arduino's baudrate and the server's listening port requirements.

STARTING THE APPLICATION
----------------

If all of the pre-requirements are met, simply 
run the Start.bat file to start the application.

Once the Start.bat file has started, allow the firewall to grant access permissions to the server .
