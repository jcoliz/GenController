using System;
using System.IO;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Common;

namespace IotHello.Uwp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application, IApplicationInfo
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

        private Platform.HardwareClock HardwareClock;

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
        public ILogger Logger => Service.TryGet<ILogger>();

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
            Logger?.LogInfo("Resuming");
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
                // Then try to start a hardware clock, and use that if available
                //
                // WARNING: This means that until Open comes back, we may not have a clock in the system!!
                Task.Run(async () => 
                {
                    try
                    {
                        // Try to create a hardware clock (DS3231), or fall back to no clock
                        var hc = await Platform.HardwareClock.Open();
                        hc.Tick();

                        Service.Set<IClock>(hc);
                        HardwareClock = hc;
                        Logger?.LogInfo("Hardware clock started.");
                    }
                    catch
                    {
                        // Nevermind, no hardware clock available
                        Logger?.LogInfo("Hardware clock failed, using built-in clock.");
                        Service.Set<IClock>(new Common.Clock());
                    }
                });

                Service.Set<ILogger>(new Common.FileSystemLogger(Windows.Storage.ApplicationData.Current.LocalFolder.Path + "\\"));
                Logger.StartSession();
                Logger.LogInfo($"{Title} {Version}");
                Service.Set<ISettings>(new Platform.WindowsSettings());
                Service.Set<IApplicationInfo>(this);

                Task.Run(async () => 
                {
                    try
                    {
                        var gen = await Platform.HardwareGenerator.Open();
                        Service.Set<Portable.Models.IGenerator>(gen);
                        Service.Set<IVoltage>(gen);
                    }
                    catch (Exception)
                    {
                        var mg = new Portable.Models.MockGenerator();
                        Service.Set<Portable.Models.IGenerator>(mg);
                        Service.Set<IVoltage>(mg);
                    }
                });

                Portable.Models.Schedule.Current.Load();

                if (Portable.Models.Schedule.Current.Periods.FirstOrDefault() == null)
                {
                    /* This is the crazy testing schedule. Once every minute!! */
                    var current = TimeSpan.FromHours(5);
                    var period = TimeSpan.FromHours(2);
                    var ending = TimeSpan.FromHours(22);
                    while (current < ending)
                    {
                        Portable.Models.Schedule.Current.Periods.Add(new Portable.Models.GenPeriod(current, current + period,0.0));
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
                                Logger?.Error("AP3", ex);
                            }
                        });

            }
            catch (Exception ex)
            {
                Logger?.Error("AP2", ex);
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
            (Portable.Models.Controller.TryCurrent as Portable.Models.Controller)?.FastTick();
        }

        private async void Timer_Tick(object sender, object e)
        {
            try
            {
                HardwareClock?.Tick();
                var t = Portable.Models.Schedule.Current.Tick();
                if (t != null)
                    await t;
                this.Tick?.Invoke(this, e);
                (Portable.Models.Controller.TryCurrent as Portable.Models.Controller)?.SlowTick();
            }
            catch (Exception ex)
            {
                Service.TryGet<ILogger>()?.Error("AP1", ex);
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            Logger?.Error("AP4", e.Exception);
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
            Logger?.LogInfo("Suspending");
            deferral.Complete();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            Logger?.LogInfo("Activated");
            base.OnActivated(args);
        }
    }
}
