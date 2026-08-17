using Microsoft.Win32;
using System;
using System.Reflection;

namespace SBOutputController
{
    public static class StartupRegistration
    {
        // Passed to the application when Windows starts it, tells it to stay hidden in the notification area
        public const string BackgroundArgument = "--background";

        private const string RunKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

#if DEBUG
        private const string ApplicationName = "SBOutputController (Debug)";
#else
        private const string ApplicationName = "SBOutputController";
#endif

        public static void Apply(bool run_on_startup)
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
            {
                if (key == null)
                {
                    return;
                }

                if (run_on_startup)
                {
                    key.SetValue(ApplicationName, string.Format("\"{0}\" {1}", Assembly.GetExecutingAssembly().Location, BackgroundArgument));
                }
                else
                {
                    key.DeleteValue(ApplicationName, false);
                }
            }
        }

        /// <summary>
        /// Adds the background argument to an entry that was registered by an older version.
        /// An entry that already has it is left alone, otherwise running a copy of the application
        /// from somewhere else would point the autostart at that copy.
        /// </summary>
        public static void UpgradeLegacyRegistration()
        {
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true))
            {
                string command = key?.GetValue(ApplicationName) as string;
                if (string.IsNullOrEmpty(command) || command.IndexOf(BackgroundArgument, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return;
                }
            }

            Apply(true);
        }

        public static bool IsBackgroundStart(string[] arguments)
        {
            return Array.Exists(arguments, argument => string.Equals(argument, BackgroundArgument, StringComparison.OrdinalIgnoreCase));
        }
    }
}
