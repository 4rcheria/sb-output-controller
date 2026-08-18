using System.Threading;
using System.Windows;

namespace SBOutputController
{
    public partial class App : Application
    {
        // Held for as long as the application runs, a second instance uses it to notice this one
        private Mutex _singleInstanceMutex;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Closing the window only hides it, the application is shut down from the notification area menu
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            _singleInstanceMutex = new Mutex(true, SingleInstance.MutexName, out bool is_only_instance);
            if (!is_only_instance)
            {
                // Hot keys can only be registered once, a second instance would come up without working
                // hot keys. Show the window of the instance that is already running instead, it may be hidden.
                SingleInstance.RequestShowWindow();
                Shutdown();
                return;
            }

            // Older versions registered a command line without the background argument
            // Qualified because Application.Properties shadows the namespace inside this class
            if (SBOutputController.Properties.Settings.Default.RunOnStartup)
            {
                StartupRegistration.UpgradeLegacyRegistration();
            }

            MainWindow main_window = new MainWindow();
            MainWindow = main_window;

            // StartInBackground fails when the setup isn't complete, in that case show the window as usual
            if (StartupRegistration.IsBackgroundStart(e.Args) && main_window.StartInBackground())
            {
                return;
            }

            main_window.Show();
        }
    }
}
