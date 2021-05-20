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
First, you need to start the Client, and to do that, you'll need to open the Scanner_Lidar.exe application.
Then, before connecting to the server, it is necessary to create a file named server.config and put it in the
following folder: C:\Users\%username%\AppData\LocalLow\DefaultCompany\Scanner_Lidar\
Inside that file, you'll need to to put the hostname of the Server and its port in the following format:
<server_ip>:<server_port>
STARTING THE APPLICATION
----------------
To start using the application, you'll need to click the "Connect to the server" button, and when the connection
is succesfully established, you can start requesting points.
