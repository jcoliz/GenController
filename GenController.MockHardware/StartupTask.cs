using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using Windows.System.Threading;
using IotFrosting.Pimoroni;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace GenController.MockHardware
{
    public sealed class StartupTask : IBackgroundTask
    {
        BackgroundTaskDeferral Deferral;
        ThreadPoolTimer Timer;
        ThreadPoolTimer StopButtonTimer;
        ThreadPoolTimer StartButtonTimer;
        Input StopButton;
        Input StartButton;
        DirectLight PanelLight;
        DirectLight RunSignalLight;
        DirectLight PowerLight;

        const int SIGNAL_START = 16;
        const int SIGNAL_POWER = 12;
        const int SIGNAL_STOP = 7;
        const int IN_STOP = 25;
        const int IN_START = 24;
        const int OUT_RUN = 23;
        const int OUT_PANEL = 18;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Deferral = taskInstance.GetDeferral();

            var StopAutoLight = new DirectLight(SIGNAL_STOP,false) { State = false };
            var StartAutoLight = new DirectLight(SIGNAL_START,false) { State = false };

            RunSignalLight = new DirectLight(OUT_RUN,false) { State = false };
            PanelLight = new DirectLight(OUT_PANEL,false) { State = false };
            PowerLight = new DirectLight(SIGNAL_POWER,false) { State = true };

            StopButton = new Input(IN_STOP,StopAutoLight,false);
            StopButton.Updated += (s, e) => 
            {
                // If pressed
                if (StopButton.State == false)
                {
                    RunSignalLight.State = false;
                    PanelLight.State = false;
                    StopButtonTimer = ThreadPoolTimer.CreateTimer((a) => 
                    {
                        PanelLight.State = true;
                    }, TimeSpan.FromSeconds(5));
                }
                else
                {
                    if (StopButtonTimer != null)
                    {
                        StopButtonTimer.Cancel();
                        StopButtonTimer = null;
                    }
                }
            };
            StartButton = new Input(IN_START,StartAutoLight,false);
            StartButton.Updated += (s, e) =>
            {
                // If pressed
                if (StartButton.State == false)
                {
                    StartButtonTimer = ThreadPoolTimer.CreateTimer((a) =>
                    {
                        RunSignalLight.State = true;
                        PanelLight.State = true;
                    }, TimeSpan.FromSeconds(5));
                }
                else
                {
                    if (StartButtonTimer != null)
                    {
                        StartButtonTimer.Cancel();
                        StartButtonTimer = null;
                    }
                }
            };

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
