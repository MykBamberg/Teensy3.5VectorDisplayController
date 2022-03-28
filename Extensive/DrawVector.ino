//Teensy Vector Monitor - Extended Syntax and Functionality

#include "kinetis.h"
#include "analog.c"

uint16_t        bufferedLines[0xf000];  //Lines to be drawn 
uint16_t        lines = 0;              //Amount of lines
char            inBuffer[9];
byte            res = 0;                //Bit resolution of the DACs

uint16_t        cursorX = 0;
uint16_t        cursorY = 0;
uint16_t        cursorMic = 5;
bool            cursorOn = false;

void setup()
{
  Serial.begin(5000000);
  //DAC "initialization" copied from analog.c (Teensyduino)
  SIM_SCGC2 |= SIM_SCGC2_DAC0;
  DAC0_C0 = DAC_C0_DACEN | DAC_C0_DACRFS;
  SIM_SCGC2 |= SIM_SCGC2_DAC1;
  DAC1_C0 = DAC_C0_DACEN | DAC_C0_DACRFS;
}

void loop()
{
  if (cursorOn)
  {
    __asm__ ("usat    %[value], #12, %[value]\n\t" : [value] "+r" (cursorX));     //makes 0 >= cursorX < 4096 (copied from analog.c)
    __asm__ ("usat    %[value], #12, %[value]\n\t" : [value] "+r" (cursorY));
    *(int16_t *)&(DAC0_DAT0L) = cursorX ;    //Set DACs
    *(int16_t *)&(DAC1_DAT0L) = cursorY ;
    delayMicroseconds(cursorMic);
  }
  if (res == 0)
  {
    for(long i = (long)lines - 1; i > -1; i--)
    {
      if(Serial.available() > 8)
      {
        serialEvent();
        i = (long)lines - 1;
      }
      line(bufferedLines[0 + 4 * i], bufferedLines[1 + 4 * i], bufferedLines[2 + 4 * i], bufferedLines[3 + 4 * i]);
    }
  }
  else
  {
    for(long i = (long)lines - 1; i > -1; i--)
    {
      if(Serial.available() > 8)
      {
        serialEvent();
        i = (long)lines - 1;
      }
      resLine(bufferedLines[0 + 4 * i], bufferedLines[1 + 4 * i], bufferedLines[2 + 4 * i], bufferedLines[3 + 4 * i]);
    }
  }
}

void serialEvent()                                      //serialEvent() gets executed, if Serial.available() at the end of loop()
{
  if(Serial.available() > 8 && Serial.peek() < 0x60)    //Reset serial buffer, if first char of the recieved message is not above 0x59 (0x20 + 0b111111)
  {
    while(Serial.available())
    {
      Serial.read();
      if(Serial.peek() >= 0x60)
      {
        break;
      }
    }
  }
  while(Serial.available() > 8 && Serial.peek() >= 0x60)
  {
    Serial.readBytes(inBuffer, 9);
    switch (inBuffer[0])
    {
      case 'c':                         //Cursor
        cursorX = inBuffer[1]  + (inBuffer[2]  << 6) - 0x820;
        cursorY = inBuffer[3]  + (inBuffer[4]  << 6) - 0x820;
        //Brightness in microseconds
        cursorMic = inBuffer[5]  + (inBuffer[6]  << 6) - 0x820;
        cursorOn = (inBuffer[7] - 0x20);
        break;

      case 'q':
        res = inBuffer[1] - 0x20; 
        break;
      
      case 'r':                         //Add rectangle
        lines *= (lines < 0x3BFC);  //Prevent overflow
        
        //Coordinates are received as "space"(0x20) + value, so 32 has to be subtracted to get the original value
        inBuffer[1] -= 0x20;      
        inBuffer[2] -= 0x20;
        inBuffer[3] -= 0x20;
        inBuffer[4] -= 0x20;
        inBuffer[5] -= 0x20;
        inBuffer[6] -= 0x20;
        inBuffer[7] -= 0x20;
        inBuffer[8] -= 0x20;
        
        bufferedLines[0 +  4 * lines] = inBuffer[5] + (inBuffer[6] << 6);
        bufferedLines[1 +  4 * lines] = inBuffer[3] + (inBuffer[4] << 6);
        bufferedLines[2 +  4 * lines] = inBuffer[1] + (inBuffer[2] << 6);
        bufferedLines[3 +  4 * lines] = inBuffer[3] + (inBuffer[4] << 6);
        
        bufferedLines[4 +  4 * lines] = inBuffer[5] + (inBuffer[6] << 6);
        bufferedLines[5 +  4 * lines] = inBuffer[7] + (inBuffer[8] << 6);
        bufferedLines[6 +  4 * lines] = inBuffer[5] + (inBuffer[6] << 6);
        bufferedLines[7 +  4 * lines] = inBuffer[3] + (inBuffer[4] << 6);
        
        bufferedLines[8 +  4 * lines] = inBuffer[1] + (inBuffer[2] << 6);
        bufferedLines[9 +  4 * lines] = inBuffer[7] + (inBuffer[8] << 6);
        bufferedLines[10 + 4 * lines] = inBuffer[5] + (inBuffer[6] << 6);
        bufferedLines[11 + 4 * lines] = inBuffer[7] + (inBuffer[8] << 6);
        
        bufferedLines[12 + 4 * lines] = inBuffer[1] + (inBuffer[2] << 6);
        bufferedLines[13 + 4 * lines] = inBuffer[3] + (inBuffer[4] << 6);
        bufferedLines[14 + 4 * lines] = inBuffer[1] + (inBuffer[2] << 6);
        bufferedLines[15 + 4 * lines] = inBuffer[7] + (inBuffer[8] << 6);
        lines += 4; 
        break;

      case 't':                         //Delete all previous lines and add rectangle
        lines = 4;
        inBuffer[1] -= 0x20;      
        inBuffer[2] -= 0x20;
        inBuffer[3] -= 0x20;
        inBuffer[4] -= 0x20;
        inBuffer[5] -= 0x20;
        inBuffer[6] -= 0x20;
        inBuffer[7] -= 0x20;
        inBuffer[8] -= 0x20;
        bufferedLines[0]  = inBuffer[5] + (inBuffer[6] << 6);
        bufferedLines[1]  = inBuffer[3] + (inBuffer[4] << 6);
        bufferedLines[2]  = inBuffer[1] + (inBuffer[2] << 6);
        bufferedLines[3]  = inBuffer[3] + (inBuffer[4] << 6);
        bufferedLines[4]  = inBuffer[5] + (inBuffer[6] << 6);
        bufferedLines[5]  = inBuffer[7] + (inBuffer[8] << 6);
        bufferedLines[6]  = inBuffer[5] + (inBuffer[6] << 6);
        bufferedLines[7]  = inBuffer[3] + (inBuffer[4] << 6);
        bufferedLines[8]  = inBuffer[1] + (inBuffer[2] << 6);
        bufferedLines[9]  = inBuffer[7] + (inBuffer[8] << 6);
        bufferedLines[10] = inBuffer[5] + (inBuffer[6] << 6);
        bufferedLines[11] = inBuffer[7] + (inBuffer[8] << 6);
        bufferedLines[12] = inBuffer[1] + (inBuffer[2] << 6);
        bufferedLines[13] = inBuffer[3] + (inBuffer[4] << 6);
        bufferedLines[14] = inBuffer[1] + (inBuffer[2] << 6);
        bufferedLines[15] = inBuffer[7] + (inBuffer[8] << 6);
        break;
        
      case '~':                         //Delete all previous lines and add line
        //0x820 = 0x20 + (0x20 << 6)
        bufferedLines[0] = inBuffer[1]  + (inBuffer[2]  << 6) - 0x820;
        bufferedLines[1] = inBuffer[3]  + (inBuffer[4]  << 6) - 0x820;
        bufferedLines[2] = inBuffer[5]  + (inBuffer[6]  << 6) - 0x820;
        bufferedLines[3] = inBuffer[7]  + (inBuffer[8]  << 6) - 0x820;
        
        lines = 1;
        break;
        
      default:                          //Add line
        lines *= (lines < 0x3BFF);
        
        bufferedLines[0 + 4 * lines] = inBuffer[1]  + (inBuffer[2]  << 6) - 0x820;
        bufferedLines[1 + 4 * lines] = inBuffer[3]  + (inBuffer[4]  << 6) - 0x820;
        bufferedLines[2 + 4 * lines] = inBuffer[5]  + (inBuffer[6]  << 6) - 0x820;
        bufferedLines[3 + 4 * lines] = inBuffer[7]  + (inBuffer[8]  << 6) - 0x820;
        
        lines++;
        break;
    }
  }
}

void line( int x0, int y0, int x1, int y1 ) 
{
  __asm__ ("usat    %[value], #12, %[value]\n\t" : [value] "+r" (x0));
  __asm__ ("usat    %[value], #12, %[value]\n\t" : [value] "+r" (y0));
  __asm__ ("usat    %[value], #12, %[value]\n\t" : [value] "+r" (x1));
  __asm__ ("usat    %[value], #12, %[value]\n\t" : [value] "+r" (y1));

  //Bresenham's line algorithm (https://en.wikipedia.org/wiki/Bresenham%27s_line_algorithm#:~:text=to%20(x0%2C%20y0).-,plotLine(x0%2C%20y0%2C%20x1%2C%20y1),-dx%20%3D%20abs(x1)
  
  int dx = abs(x1 - x0);
  int sx = x0 < x1 ? 1 : -1;
  int dy = -abs(y1 - y0);
  int sy = y0 < y1 ? 1 : -1;
  int err = dx + dy;
  int e2;
  
  while (x0 != x1 or y0 != y1) 
  {
    *(int16_t *)&(DAC0_DAT0L) = x0;
    *(int16_t *)&(DAC1_DAT0L) = y0;
    e2 = err << 1;
    err += dy * (e2 > dy);
    x0  += sx * (e2 > dy);
    err += dx * (e2 < dx);
    y0  += sy * (e2 < dx);
  }
}

void resLine( int x0, int y0, int x1, int y1 ) 
{
  __asm__ ("usat    %[value], #12, %[value]\n\t" : [value] "+r" (x0));
  __asm__ ("usat    %[value], #12, %[value]\n\t" : [value] "+r" (y0));
  __asm__ ("usat    %[value], #12, %[value]\n\t" : [value] "+r" (x1));
  __asm__ ("usat    %[value], #12, %[value]\n\t" : [value] "+r" (y1));

  x0 = x0 >> res;
  x1 = x1 >> res;
  y0 = y0 >> res;
  y1 = y1 >> res;
  
  int dx = abs(x1 - x0);
  int sx = x0 < x1 ? 1 : -1;
  int dy = -abs(y1 - y0);
  int sy = y0 < y1 ? 1 : -1;
  int err = dx + dy;
  int e2;
  
  while (x0 != x1 or y0 != y1) 
  {
    *(int16_t *)&(DAC0_DAT0L) = x0 << res;
    *(int16_t *)&(DAC1_DAT0L) = y0 << res;
    e2 = err << 1;
    err += dy * (e2 > dy);
    x0  += sx * (e2 > dy);
    err += dx * (e2 < dx);
    y0  += sy * (e2 < dx);
  }
}
