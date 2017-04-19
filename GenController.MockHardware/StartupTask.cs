using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;
using Pimoroni.MsIot;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace GenController.MockHardware
{
    public sealed class StartupTask : IBackgroundTask
    {
        BackgroundTaskDeferral Deferral;
        ThreadPoolTimer Timer;
        Input StopButton;
        Input StartButton;

        const int run_signal_pin = 24;
        const int panel_light_pin = 25;
        const int start_autolight_pin = 23;
        const int stop_autolight_pin = 18;
        const int stop_button_pin = 20;
        const int start_button_pin = 21;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Deferral = taskInstance.GetDeferral();

            var StopAutoLight = new DirectLight(stop_autolight_pin);
            var StartAutoLight = new DirectLight(start_autolight_pin);

            StopButton = new Input(stop_button_pin,StopAutoLight,false);
            StopButton.Updated += (s, e) => 
            {

            };
            StartButton = new Input(start_button_pin,StartAutoLight,false);

            Timer = ThreadPoolTimer.CreatePeriodicTimer(Tick, TimeSpan.FromMilliseconds(100));
        }

        private void Tick(ThreadPoolTimer timer)
        {
            // Maintain the autolight for the start/stop buttons
            StopButton.Tick();
            StartButton.Tick();
        }
    }
}
