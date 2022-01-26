# Teensy3.5VectorDisplayController
This code allows you to use an Oscilloscope with an X-Y-mode as a vector display.

---
How to use:

Step 1:  
  Compile and upload "DrawVector.ino" to a Teensy 3.5 or 3.6 using Arduino (Teesyduino).  
Step 2:  
  Add "VectorOutput" to your C# project.  
Step 3:  
  Execute "VectorOutput.GetComPort();" at the beginning of your program and save its result to a string.  
  (When starting the program, a window asking you to select a COM port should pop up)  
Step 4:  
  To update the image on the oscilloscope use "VectorOutput.DrawFrame(int[][] frame, string comPort)".  
  The jagged array "frame" should contain at least one line. A line is an int array whit length 4.  
  The first number is the X-coordinate of the beginning of the line,  
  the second the y-coordinate of the beginning of the line,  
  the 3rd is the X-coordinate of the line ending and  
  the 3rd is the Y-coordinate of the line ending.  
Step 5:  
  Select the COM-port of your Teensy when starting the program.  
