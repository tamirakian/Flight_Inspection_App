# Flight_Inspection_App
Flight_Inspection_App
Contributes:
Tamir Akian , Gabriel Cembal , Noam Lachmani , Sapir Shafsha
Description:
"Flight_Inspection_App" is an application which is designed for pilots and flight researchers.
This application allows to track anomalies discovered during the flight.

First time running:
1) download the flight gear simulator.
You can download it in the link below:
http://www.flightgear.org

2) Insert the xaml file in the protocol directory which is located in the flight gear installed directory under data:
C:\Program Files\FlightGear 2020.3.6\data\Protocol

3) Run the Flight gear simulator and choose the data directory in the given box. The path is:
C:\Program Files\FlightGear 2020.3.6\data
Restart the simulator.

4) In the Flight gear simulator go to the settings, in the command line below type:
--generic=socket,in,10,127.0.0.1,5400,tcp,playback_small
--fdm=null


Using the Flight_Inspection_App:
When the user runs the program, a window for importing files
Will be displayed.
The user must import the following files: 
*A Normal Flight – a csv file. This flight contains no anomalies.
* An Anomaly Flight -a csv file.
* FlightGear settings –a xml flie.

** Every 10 lines in the CSV file represents one second of flight.
Afterwards the user must type the path to the .exe of the Flight gear simulator in the box which will appear on screen.
-There is a media player GUI which works similarly to a generic media player.
Playback Panel - you can start and pause the flight. In addition you can move back and forth (each click moves the flight for a second forward or backward).
Furthermore, you can enter a desired speed of the projection of the flight video, the speed can get values between 0 and 3.

Graphs and list – the list contain several features of the flight. When you click on a feature, you can see in the graph a line that changes  and represents the values of the flight in a given time.
The top right graph displays the selected value, 
The top left graph displays the correlative graph,
The bottom graph displays the regression line.

On the top right of the screen there is a list of features that change dynamically during the flight.

Joystick- The joystick is bound to the values of the elevator and the aileron. It moves alone with the change of the values.

Folder Hierarchy
HelperClasses 
Models 
ViewModels
WindowObjects – Contains the display windows as a screen for loading files and user stories.
MainWindow- The opening screen from which the user can start importing the files from.


Documentation
In the following link you will find documentation on the UML parts and the connections between the various departments.

The link to the UML:
https://files.fm/u/mxn4bzts5

The link to the explenation video:
https://drive.google.com/file/d/1uSQHJNocU3cxB_KobmltdSNDI14UbCPU/view




