using System.Windows;

namespace SBOutputController
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Closing the window only hides it, the application is shut down from the notification area menu
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

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
