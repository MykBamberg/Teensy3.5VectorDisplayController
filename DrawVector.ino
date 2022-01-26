char            inBuffer[14];
byte            inPos = 0;

uint16_t        bufferedLines[0xf000];
uint16_t        lines = 0;
uint32_t        pathLengh;
byte            res = 1;

#define         xpin  A21
#define         ypin  A22

void setup()
{
  analogWriteResolution(12);
  Serial.begin(5000000);
}

void loop()
{
  for(long i = (long)lines - 1; i > -1; i--)
  {
    if(Serial.available() > 0)
    {
      serialEvent();
      break;
    }
    line(bufferedLines[0 + 4 * i], bufferedLines[1 + 4 * i], bufferedLines[2 + 4 * i], bufferedLines[3 + 4 * i]);
  }
}

void serialEvent()
{
  while(Serial.available())
  {
    inBuffer[inPos] = Serial.read();
    
    inPos++;
    inPos *= (inBuffer[0] == '|');
    if (inPos > 13)
    {
      lines *= (inBuffer[13] != '~') * (pathLengh < 0b10000000000000000000);
      pathLengh *= (lines != 0);
      for (byte i = 1; i < 13; i++)
      {
        inBuffer[i] =   (inBuffer[i] >= '1') +
                        (inBuffer[i] >= '2') +
                        (inBuffer[i] >= '3') +
                        (inBuffer[i] >= '4') +
                        (inBuffer[i] >= '5') +
                        (inBuffer[i] >= '6') +
                        (inBuffer[i] >= '7') +
                        (inBuffer[i] >= '8') +
                        (inBuffer[i] >= '9') +
                        (inBuffer[i] >= 'A') +
                        (inBuffer[i] >= 'B') +
                        (inBuffer[i] >= 'C') +
                        (inBuffer[i] >= 'D') +
                        (inBuffer[i] >= 'E') +
                        (inBuffer[i] >= 'F');
                        
      }
      bufferedLines[0 + 4 * lines] = inBuffer[3]  + (inBuffer[2]  << 4) + (inBuffer[1]  << 8);
      bufferedLines[1 + 4 * lines] = inBuffer[6]  + (inBuffer[5]  << 4) + (inBuffer[4]  << 8);
      bufferedLines[2 + 4 * lines] = inBuffer[9]  + (inBuffer[8]  << 4) + (inBuffer[7]  << 8);
      bufferedLines[3 + 4 * lines] = inBuffer[12] + (inBuffer[11] << 4) + (inBuffer[10] << 8);
      
      pathLengh +=  abs(bufferedLines[0 + 4 * lines] - 
                        bufferedLines[2 + 4 * lines])+ 
                    abs(bufferedLines[1 + 4 * lines] - 
                        bufferedLines[3 + 4 * lines])+
                        0x60;
      lines++;
      inPos = 0;
    }
  }
  res = ((pathLengh >> 14) + 1);
}

void line(long x0, long y0, long x1, long y1 ) 
{
  x0 = x0 / res;
  x1 = x1 / res;
  y0 = y0 / res;
  y1 = y1 / res;
  
  long dx = abs(x1 - x0);
  long sx = x0 < x1 ? 1 : -1;
  long dy = -abs(y1 - y0);
  long sy = y0 < y1 ? 1 : -1;
  long err = dx + dy;
  long e2;
  
  while (x0 != x1 or y0 != y1) 
  {
    analogWrite(xpin, x0 * res);
    analogWrite(ypin, y0 * res);
    e2 = err << 2;
    err += dy * (e2 > dy);
    x0 += sx * (e2 > dy);
    err += dx * (e2 < dx);
    y0 += sy * (e2 < dx);
  }
}
