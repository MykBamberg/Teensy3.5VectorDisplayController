# Teensy3.5VectorDisplayController
This code allows you to use a Teensy 3.5 and a vectorscope (oscilloscope on X-Y-mode) as a vector display.

## Simple/Extensive
The "Simple" folder contains a simplified version of the display controller firmware and a C# file which includes the bare minimum that is required to display an image. The "Extensive" folder contains a more advanced firmware and a NuGet package that allows fluid animations and multiple other features like a cursor, sprites, stamps, and resolution control.

# Firmware Installation
Note: Although the file is called "DrawVector.ino" in both cases, the firmware is not interchangeable between "Simple" and "Extensive"

1. Install the [Arduino IDE](https://www.arduino.cc/en/software) and [Teensyduino](https://www.pjrc.com/teensy/teensyduino.html)
2. Extract DrawVector.ino and open it with the Arduino IDE (If it asks you to create a sketch folder, you have to accept)
3. Make sure that the Teesny 3.5 is connected and select it and its COM port under the "Tools" tab in the Arduino IDE
4. Hit the upload button and wait for the code to compile and upload

# Hardware Configuration
Connect the pin labeled "DAC0" to the vektorscope's "X"-channel and the pin labeled "DAC1" to the "Y"-Channel. Afterward, you have to connect the Teesny's ground with the vectorscope's ground. ([Teensy 3.5 pinout](https://www.pjrc.com/teensy/card8a_rev3_web.pdf))

# NuGet Package
[NuGet-Release](https://www.nuget.org/packages/VectorOutput/1.0.0)

Package Manager: ```Install-Package VectorOutput -Version 1.0.0```

# Usage
Create a global ```VectorOutput``` variable:
```cs
private VectorOutput vOut;
```
Request the COM port of your Teesny and create a new ```VectorOutput``` instance:
```cs
string? comPort = VectorOutput.ComPortForm();
while (comPort == null)
{
    comPort = VectorOutput.ComPortForm();
}

try
{
    vOut = new VectorOutput(comPort);
}
catch (Exception ex)
{
    MessageBox.Show(ex.Message);
}
```
Display an image:
```cs
vOut.Lines = new ushort[][] { new ushort[] { 0, 0, 0xFFF, 0xFFF } };
vOut.DrawFrame();
```
Note: ```Lines``` is a jagged array of ushort arrays. The ushort arrays represent lines and always have a length of 4. The first two ushorts are the starting X and Y coordinates of the line, the last two are the ending X and Y. All coordinates have to be between 0 (bottom/left) and 4095/0xFFF (top/right).
