# Teensy3.5VectorDisplayController
This code allows you to use an Oscilloscope with an X-Y-mode as a vector display.

---
How to use:

Step 1:__
  Compile and upload "DrawVector.ino" to a Teensy 3.5 or 3.6 using Arduino (Teesyduino).__
Step 2:__
  Add "VectorOutput" to your C# project.__
Step 3:__
  Execute "VectorOutput.GetComPort();" at the beginning of your program and save its result to a string.__
  (When starting the program, a window asking you to select a COM port should pop up)__
Step 4:__
  To update the image on the oscilloscope use "VectorOutput.DrawFrame(int[][] frame, string comPort)".__
  The jagged array "frame" should contain at least one line. A line is an int array whit length 4. __
  The first number is the X-coordinate of the beginning of the line, __
  the second the y-coordinate of the beginning of the line, __
  the 3rd is the X-coordinate of the line ending and __
  the 3rd is the Y-coordinate of the line ending.__
Step 5:__
  Select the COM-port of your Teensy when starting the program.__
