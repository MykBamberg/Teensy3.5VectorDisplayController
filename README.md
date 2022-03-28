[outdated; will be completely rewritten as soon as the nuget package will be avalible on https://www.nuget.org/packages/VectorOutput/1.0.0]

# Teensy3.5VectorDisplayController
This code allows you to use a Teensy 3.5 (3.6 would probably work to but I haven't tested it) and a vectorscope (oscilloscope with an X-Y-mode) as a vector display.

---
How to use:

- [ ] Step 1:  
		Compile and upload "DrawVector.ino" to a Teensy 3.5 or 3.6 using Arduino (Teesyduino).  
- [ ] Step 2:  
		Add "VectorOutput" to your C# project.  
- [ ] Step 3:  
		Execute "VectorOutput.GetComPort();" at the beginning of your program and save its result to a string.  
		(When starting the program, a window asking you to select a COM port should pop up)  
- [ ] Step 4:  
		To update the image on the oscilloscope use "VectorOutput.DrawFrame(int[][] frame, string comPort)".  
		The jagged array "frame" should contain at least one line. A line is an int array whit length 4.  
		The first number is the X-coordinate of the beginning of the line,  
		the second the y-coordinate of the beginning of the line,  
		the 3rd is the X-coordinate of the line ending, and  
		the 3rd is the Y-coordinate of the line ending.  
- [ ] Step 5:  
		Connect the DAC0 and DAC1 pins of your Teensy to an oscilloscope.  
- [ ] Step 6:  
		Select the COM-port of your Teensy when starting the program.  

Note: You'll need to reference "System.IO.Ports" and "WindowsForms" in your project! 
