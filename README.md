# Teensy3.5VectorDisplayController
This code allows you to use a Teensy 3.5 and a vectorscope (oscilloscope on X-Y-mode) as a vector display.

# Firmware Installation

1. Install the [Arduino IDE](https://www.arduino.cc/en/software) and [Teensyduino](https://www.pjrc.com/teensy/teensyduino.html) ([Arduino IDE with Teensyduino for Arch based systems](https://aur.archlinux.org/packages/teensyduino))
2. Extract DrawVector and open DrawVector.ino in the Arduino IDE
3. Make sure that the Teensy 3.5 is connected and select it and its serial port under the "Tools" tab in the Arduino IDE
4. Hit the upload button and wait for the code to compile and upload

# Hardware Configuration
Connect the pin labeled "DAC0" to the vectorscope's "X"-channel and the pin labeled "DAC1" to the "Y"-Channel. Afterward, you have to connect the Teensy's ground with the vectorscope's ground. ([Teensy 3.5 pinout](https://www.pjrc.com/teensy/card8a_rev3_web.pdf))

# NuGet Package
[NuGet-Release](https://www.nuget.org/packages/VectorOutput/)

Package Manager: ```Install-Package VectorOutput -Version 1.1.3```

# Usage
Create a global ```VectorOutput``` variable:
```cs
private VectorOutput vOut;
```
Request the COM port of your Teensy and create a new ```VectorOutput``` instance:
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
