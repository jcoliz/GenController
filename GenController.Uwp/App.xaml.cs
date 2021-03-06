﻿using System;
using System.Linq;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Commonality;
using GenController.Portable.Models;

namespace GenController.Uwp
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application, IApplicationInfo
    {
        /// <summary>
        /// Will be raised every second. Attach anything to this you want updated
        /// regularly.
        /// </summary>
        public event EventHandler<object> Tick;

        /// <summary>
        /// Reference to the singleton app instance
        /// </summary>
        public static new App Current { get; private set; }

        /// <summary>
        /// Timer to run scheduled program logic and UI updates
        /// </summary>
        private DispatcherTimer Timer;

        /// <summary>
        /// High-resolutoin timer only for hardware timing
        /// </summary>
        private ThreadPoolTimer HardwareTimer;

        /// <summary>
        /// The hardware clock we are using to keep time. Null if we are using
        /// a software clock
        /// </summary>
        private Platform.HardwareClock HardwareClock;

        /// <summary>
        /// Task running our HTTPD server
        /// </summary>
        private static IAsyncAction ServerTask;

        /// <summary>
        /// The current schedule we are follwoing
        /// </summary>
        private static Schedule ScheduleCurrent;

        /// <summary>
        /// The current logic controller
        /// </summary>
        public static Controller ControllerCurrent;

        private static RemoteControlLogic RemoteControlCurrent;

        private Catnap.Server.HttpServer httpServer;

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
            Logger?.LogInfoAsync("Resuming");
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override async void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif
            try
            {
                var settings = new Platform.WindowsSettings();
                Service.Set<ISettings>(settings);

                IClock clock = null;
                try
                {
                    // Try to create a hardware clock (DS3231), or fall back to no clock
                    var hc = await Platform.HardwareClock.Open();
                    hc.Tick();

                    Service.Set<IClock>(hc);
                    clock = HardwareClock = hc;

                    // No logger yet!!
                    //await Logger?.LogInfoAsync("Hardware clock started.");
                }
                catch
                {
                    // Nevermind, no hardware clock available
                    // There is no logger yet!
                    //await Logger?.LogInfoAsync("Hardware clock failed, using built-in clock.");

                    // We'll use a software clock instead
                    Service.Set<IClock>(clock = new Clock(settings));
                }

                // Set up services to be located by other components
                Service.Set<ILogger>(new Portable.Models.FileSystemLoggerWithVoltage(clock,Windows.Storage.ApplicationData.Current.LocalFolder.Path + "\\"));
                Service.Set<IApplicationInfo>(this);

                await Logger.StartSessionAsync();
                await Logger.LogInfoAsync($"{Title} {Version}");

                try
                {
                    // Try to open a connection to the hardware generator line
                    var gen = await Platform.HardwareGenerator.Open();
                    Service.Set<Portable.Models.IGenerator>(gen);
                    Service.Set<Portable.Models.IVoltage>(gen);
                }
                catch (Exception)
                {
                    // If there is no hardware generator controller, we'll mock it up in
                    // sofware. This is helpful for UI and logic testing.
                    var mg = new Portable.Models.MockGenerator();
                    Service.Set<Portable.Models.IGenerator>(mg);
                    Service.Set<Portable.Models.IVoltage>(mg);
                }

                ScheduleCurrent = new Schedule();
                Commonality.Service.Set<ISchedule>(ScheduleCurrent);
                ScheduleCurrent.Load();

                if (ScheduleCurrent.Periods.FirstOrDefault() == null)
                {
                    var current = TimeSpan.FromHours(5);
                    var period = TimeSpan.FromHours(2);
                    var ending = TimeSpan.FromHours(22);
                    while (current < ending)
                    {
                        ScheduleCurrent.Periods.Add(new GenPeriod(current, current + period,0.0));
                        current += period + period;
                    }
                }

                ControllerCurrent = new Controller();
                Service.Set<IController>(ControllerCurrent);

                // Try to open the hardware remote 
                try
                {
                    var remote = new Platform.HardwareRemote();
                    RemoteControlCurrent = new RemoteControlLogic(remote, ControllerCurrent);
                }
                catch
                {
                    // OK if it fails. We just won't get any inputs from the remote
                    await Logger.LogInfoAsync("No hardware remote.");
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
                                ex.Source = "AP3";
                                if (Logger != null)
                                    await Logger.LogErrorAsync(ex);
                            }
                        });
               
            }
            catch (Exception ex)
            {
                ex.Source = "AP2";
                if (Logger != null)
                    await Logger.LogErrorAsync(ex);
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
            ControllerCurrent.FastTick();
        }

        private async void Timer_Tick(object sender, object e)
        {
            try
            {
                HardwareClock?.Tick();
                var t = ScheduleCurrent.Tick();
                if (t != null)
                    await t;
                this.Tick?.Invoke(this, e);
                ControllerCurrent.SlowTick();
            }
            catch (Exception ex)
            {
                ex.Source = "AP1";
                Logger?.LogErrorAsync(ex);
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            e.Exception.Source = "AP4";
            Logger?.LogErrorAsync(e.Exception);
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
            Logger?.LogInfoAsync("Suspending");
            deferral.Complete();
        }

        protected override void OnActivated(IActivatedEventArgs args)
        {
            Logger?.LogInfoAsync("Activated");
            base.OnActivated(args);
        }
    }
}
