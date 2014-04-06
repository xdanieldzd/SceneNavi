using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace SceneNavi.OpenGLHelpers
{
    // Adapted from http://www.daniweb.com/software-development/csharp/code/408351/xna-framework-get-frames-per-second#post1743620
    /// <summary>
    /// Stopwatch-based FPS counter
    /// </summary>
    public class FPSMonitor
    {
        public float Value { get; private set; }
        public TimeSpan Sample { get; set; }

        Stopwatch sw;
        int frames;

        public FPSMonitor()
        {
            Sample = TimeSpan.FromSeconds(1);
            Value = 0;
            frames = 0;
            sw = Stopwatch.StartNew();
        }

        public void Update()
        {
            frames++;

            if (sw.Elapsed > Sample)
            {
                Value = (float)(frames / sw.Elapsed.TotalSeconds);

                sw.Reset();
                sw.Start();
                frames = 0;
            }
        }
    }
}
