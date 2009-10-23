using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace VirtualBicycle.Graphics
{
    public class FpsCounter
    {
        long begin, end;
        float fps = -1;

        long stat;
        float multiplier = 1;

        int statMax = 100;

        Stopwatch sw;

        public FpsCounter()
        {
            sw = Stopwatch.StartNew();
            SetBegin();
        }

        public int StepFrame
        {
            get { return statMax; }
            set { statMax = value; }
        }
        public float FPS
        {
            get { return fps; }
        }
        public float Multiplier
        {
            get { return multiplier; }
            set { multiplier = value; }
        }

        public void Update()
        {
            stat++;

            if (stat >= statMax)
            {
                end = sw.ElapsedMilliseconds;// Environment.TickCount;

                fps = 1000.0f * ((float)stat / (float)(end - begin));
                stat = 0;
                SetBegin();
            }
        }
        void SetBegin()
        {
            begin = sw.ElapsedMilliseconds;// Environment.TickCount;
        }

        public override string ToString()
        {
            return "FPS= " + System.Math.Round(fps * multiplier, 2).ToString();
        }
    }
}
