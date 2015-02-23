RWTexture2D<float4> Output;

void Plot(int x, int y)
{
   Output[uint2(x, y)] = float4(0, 0, 1, 1);
}

void DrawLine(int2 start, int2 end)
{
     bool steep = abs(end.y - start.y) > abs(end.x - start.x);

     if (steep)
     {
         int temp = start.x;
         start.x = start.y;
         start.y = temp;
         
         temp = end.x;
         end.x = end.y;
         end.y = temp;
     }

     if (start.x > end.x)
     {
         int temp = start.x;
         start.x = end.x;
         end.x = temp;
         
         temp = start.y;
         start.y = end.y;
         end.y = temp;
     }

     int deltax = end.x - start.x;
     int deltay = abs(end.y - start.y);
     float error = 0;
     float deltaerr = deltay / deltax;
     int ystep;
     int y = start.y;

     if (start.y < end.y)
        ystep = 1; 
     else 
        ystep = -1;

     for (int x = start.x; x < end.x; x++)
     {
         if (steep)
            Plot(y, x);
         else 
            Plot(x, y);

         error = error + deltaerr;

         if (error >= 0.5)
         {
             y = y + ystep;
             error = error - 1.0;
         }
    }
}  

void DrawCircle(int x0, int y0, int radius)
{
  int f = 1 - radius;
  int ddF_x = 1;
  int ddF_y = -2 * radius;
  int x = 0;
  int y = radius;
 
  Plot(x0, y0 + radius);
  Plot(x0, y0 - radius);
  Plot(x0 + radius, y0);
  Plot(x0 - radius, y0);
 
  while(x < y)
  {
    // ddF_x == 2 * x + 1;
    // ddF_y == -2 * y;
    // f == x*x + y*y - radius*radius + 2*x - y + 1;
    if(f >= 0) 
    {
      y--;
      ddF_y += 2;
      f += ddF_y;
    }
    x++;
    ddF_x += 2;
    f += ddF_x;    
    Plot(x0 + x, y0 + y);
    Plot(x0 - x, y0 + y);
    Plot(x0 + x, y0 - y);
    Plot(x0 - x, y0 - y);
    Plot(x0 + y, y0 + x);
    Plot(x0 - y, y0 + x);
    Plot(x0 + y, y0 - x);
    Plot(x0 - y, y0 - x);
  }
}

/*
void DDA(int xa, int ya, int xb, int yb)
{
        int x;
        float dydx = (float) (yb - ya) / (float) (xb - xa);
        float y = ya;
        for (x=xa; x<=xb; x++) {
                Plot(x, round(y));
                y = y + dydx;
        }
}
*/

void DDA(float2 start, float2 end)
{
    float dydx = (end.y - start.y) / (end.x - start.x);
    float y = start.y;
    for (int x = start.x; x <= end.x; x++) 
    {
        Plot(x, round(y));
        y = y + dydx;
    }
}

[numthreads(32, 32, 1)]
void main( uint3 threadID : SV_DispatchThreadID )
{
    Output[threadID.xy] = float4(threadID.xy / 1024.0f, 0, 1);
    
    //DrawLine(int2(0, 0), int2(100, 100));
    //DrawLine(int2(100, 100), int2(0, 0));
    
    //DrawLine(int2(0, 100), int2(100, 0));
    //DrawLine(int2(100, 0), int2(0, 100));
    
    //DDA(0, 0, 100, 100);
    //DDA(0, 99, 99, 0);
    
    if (threadID.x == 1023 && threadID.y == 1023)
    {
		DDA(float2(0, 0), float2(1024, 1024));
		DDA(float2(0, 1023), float2(1023, 0));
	    
		DrawCircle(512, 512, 250);
		DrawCircle(0, 512, 250);
    }
}