using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Threading;

namespace YTR.Classes
{    public class UIBlockDetector
    {
        static System.Threading.Timer _timer;
        public UIBlockDetector(int maxFreezeTimeInMilliseconds = 200)
        {
            var sw = new Stopwatch();

            new DispatcherTimer(TimeSpan.FromMilliseconds(10), DispatcherPriority.Send, (sender, args) =>
            {
                lock (sw)
                {
                    sw.Restart();
                }

            }, Dispatcher.CurrentDispatcher );

            _timer = new System.Threading.Timer(state =>
            {
                lock (sw)
                {
                    if (sw.ElapsedMilliseconds > maxFreezeTimeInMilliseconds)
                    {
                        //Debugger.Break();
                        // Debugger.Break() or set breakpoint here;
                        // Goto Visual Studio --> Debug --> Windows --> Theads 
                        // and checkup where the MainThread is.
                    }
                }

            }, null, TimeSpan.FromMilliseconds(0), TimeSpan.FromMilliseconds(10));

        }

    }
}
