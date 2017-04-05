using ManiaLabs.Models;
using ManiaLabs.Portable.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;

namespace IotHello.Uwp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application, ManiaLabs.IApp
    {
        /// <summary>
        /// Timer to run scheduled program logic and UI updates
        /// </summary>
        private DispatcherTimer Timer;

        public event EventHandler<object> Tick;

        /// <summary>
        /// High-resolutoin timer only for hardware timing
        /// </summary>
        private ThreadPoolTimer HardwareTimer;

        /// <summary>
        /// Task running our HTTPD server
        /// </summary>
        private static IAsyncAction ServerTask;

        private Catnap.Server.HttpServer httpServer;

        public static new App Current { get; private set; }

        public string Title
        {
            get
            {
                var package = Package.Current;

                return package?.DisplayName ?? string.Empty;
            }
        }
        public string Version
        {
            get
            {
                var package = Package.Current;
                var packageid = package?.Id;
                var version = packageid?.Version;

                var result = string.Empty;
                if (version != null)
                {
                    result = $"{version.Value.Major}.{version.Value.Minor}.{version.Value.Build}";
                }

                return result;
            }
        }
        public IMeasurement Measurement => ManiaLabs.Platform.Get<IMeasurement>();

        public string DeviceUniqueID
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string PurchasesInfo
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public string Launches
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.Resuming += App_Resuming;
            Current = this;
        }

        private void App_Resuming(object sender, object e)
        {
            ManiaLabs.Platform.TryGet<IMeasurement>()?.LogInfo("Resuming");
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            try
            {
                ManiaLabs.Platform.Set<ManiaLabs.IApp>(this);
                ManiaLabs.Platform.Set<IClock>(new ManiaLabs.NET.Clock());
                ManiaLabs.Platform.Set<IPlatformFilesystem>(new ManiaLabs.DotNetPlatform.DotNetFileSystem(Windows.Storage.ApplicationData.Current.LocalFolder.Path + "\\"));
                ManiaLabs.Platform.Set<IMeasurement>(new SimpleMeasurement());
                ManiaLabs.Platform.Get<IMeasurement>().StartSession();
                ManiaLabs.Platform.Get<IMeasurement>().LogInfo($"{Title} {Version}");
                ManiaLabs.Platform.Set<IPlatformSettingsManager>(new Platform.WindowsSettingsManager());
                ManiaLabs.Platform.Set<Portable.Models.IGenerator>(new Portable.Models.MockGenerator());

                /* This is the REAL schedule
                Portable.Models.Schedule.Current.Periods.Add(new Portable.Models.GenPeriod(TimeSpan.FromHours(7), TimeSpan.FromHours(9)));
                Portable.Models.Schedule.Current.Periods.Add(new Portable.Models.GenPeriod(TimeSpan.FromHours(12), TimeSpan.FromHours(14)));
                Portable.Models.Schedule.Current.Periods.Add(new Portable.Models.GenPeriod(TimeSpan.FromHours(17), TimeSpan.FromHours(19)));
                */

                Portable.Models.Schedule.Current.Load();

                if (Portable.Models.Schedule.Current.Periods.FirstOrDefault() == null)
                {
                    /* This is the crazy testing schedule. Once every minute!! */
                    var current = TimeSpan.FromHours(5);
                    var period = TimeSpan.FromHours(2);
                    var ending = TimeSpan.FromHours(22);
                    while (current < ending)
                    {
                        Portable.Models.Schedule.Current.Periods.Add(new Portable.Models.GenPeriod(current, current + period));
                        current += period + period;
                    }
                }

                Timer = new DispatcherTimer() { Interval = TimeSpan.FromSeconds(1) };
                Timer.Tick += Timer_Tick;
                Timer.Start();

                HardwareTimer = ThreadPoolTimer.CreatePeriodicTimer(HardwareTick, TimeSpan.FromMilliseconds(50));
                
                httpServer = new Catnap.Server.HttpServer(1339);
                httpServer.restHandler.RegisterController(new Controllers.StatusController());
                httpServer.restHandler.RegisterController(new Controllers.LogsController());

                ServerTask =
                        ThreadPool.RunAsync(async (w) =>
                        {
                            try
                            {
                                await httpServer.StartServer();
                            }
                            catch (Exception ex)
                            {
                                ManiaLabs.Platform.TryGet<IMeasurement>()?.Error("AP3", ex);
                            }
                        });

            }
            catch (Exception ex)
            {
                ManiaLabs.Platform.TryGet<IMeasurement>()?.Error("AP2", ex);
            }

            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (e.PrelaunchActivated == false)
            {
                if (rootFrame.Content == null)
                {
                    // When the navigation stack isn't restored navigate to the first page,
                    // configuring the new page by passing required information as a navigation
                    // parameter
                    rootFrame.Navigate(typeof(MainPage), e.Arguments);
                }
                // Ensure the current window is active
                Window.Current.Activate();
            }
        }

        /// <summary>
        /// Execute the high-resolution timer for hardware timing
        /// </summary>
        /// <param name="timer"></param>
        private void HardwareTick(ThreadPoolTimer timer)
        {
            (Portable.Models.Controller.TryCurrent as Portable.Models.Controller)?.HardwareTick();
        }

        private async void Timer_Tick(object sender, object e)
        {
            try
            {
                var t = Portable.Models.Schedule.Current.Tick();
                if (t != null)
                    await t;
                this.Tick?.Invoke(this, e);
            }
            catch (Exception ex)
            {
                ManiaLabs.Platform.TryGet<IMeasurement>()?.Error("AP1", ex);
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            ManiaLabs.Platform.TryGet<IMeasurement>()?.Error("AP4", e.Exception);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            ManiaLabs.Platform.TryGet<IMeasurement>()?.LogInfo("Suspending");
            deferral.Complete();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            ManiaLabs.Platform.TryGet<IMeasurement>()?.LogInfo("Activated");
            base.OnActivated(args);
        }

        public void GoBack()
        {
            throw new NotImplementedException();
        }

        public string GetResourceString(string key)
        {
            return key;
        }

        public Task<Stream> OpenStreamForReadFromApplicationAsync(string Pathname)
        {
            throw new NotImplementedException();
        }
    }
}
