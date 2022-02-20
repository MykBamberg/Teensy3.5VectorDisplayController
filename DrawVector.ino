#include "kinetis.h"
#include "analog.c"

uint16_t        bufferedLines[0xf000];
uint16_t        lines = 0;
char            inBuffer[9];

void setup()
{
  Serial.begin(5000000);
  SIM_SCGC2 |= SIM_SCGC2_DAC0;
  DAC0_C0 = DAC_C0_DACEN | DAC_C0_DACRFS;
  SIM_SCGC2 |= SIM_SCGC2_DAC1;
  DAC1_C0 = DAC_C0_DACEN | DAC_C0_DACRFS;
}

void loop()
{
  for(long i = (long)lines - 1; i > -1; i--)
  {
    if(Serial.available() > 8)
    {
      serialEvent();
      break;
    }
    line(bufferedLines[0 + 4 * i], bufferedLines[1 + 4 * i], bufferedLines[2 + 4 * i], bufferedLines[3 + 4 * i]);
  }
}

void serialEvent()
{
  if(Serial.peek() < 60)
  {
    while(Serial.available())
    {
      Serial.read();
      if(Serial.peek() >= 60)
      {
        break;
      }
    }
  }
  while(Serial.available() > 8)
  {
    Serial.readBytes(inBuffer, 9);
    lines *= (inBuffer[0] != '~') * (lines < 0x3BFF);
    bufferedLines[0 + 4 * lines] = inBuffer[1]  + (inBuffer[2]  << 6) - 0x820;
    bufferedLines[1 + 4 * lines] = inBuffer[3]  + (inBuffer[4]  << 6) - 0x820;
    bufferedLines[2 + 4 * lines] = inBuffer[5]  + (inBuffer[6]  << 6) - 0x820;
    bufferedLines[3 + 4 * lines] = inBuffer[7]  + (inBuffer[8]  << 6) - 0x820;
    lines++;
  }
}

void line(int x0, int y0, int x1, int y1 ) 
{
  __asm__ ("usat    %[value], #12, %[value]\n\t" : [value] "+r" (x0));
  __asm__ ("usat    %[value], #12, %[value]\n\t" : [value] "+r" (y0));
  __asm__ ("usat    %[value], #12, %[value]\n\t" : [value] "+r" (x1));
  __asm__ ("usat    %[value], #12, %[value]\n\t" : [value] "+r" (y1));
  
  int dx = abs(x1 - x0);
  int sx = x0 < x1 ? 1 : -1;
  int dy = -abs(y1 - y0);
  int sy = y0 < y1 ? 1 : -1;
  int err = dx + dy;
  int e2;
  
  while (x0 != x1 or y0 != y1) 
  {
    *(int16_t *)&(DAC0_DAT0L) = x0 ;
    *(int16_t *)&(DAC1_DAT0L) = y0 ;
    e2 = err << 1;
    err += dy * (e2 > dy);
    x0  += sx * (e2 > dy);
    err += dx * (e2 < dx);
    y0  += sy * (e2 < dx);
  }
}
