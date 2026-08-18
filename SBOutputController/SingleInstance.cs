using System;
using System.Runtime.InteropServices;

namespace SBOutputController
{
    public static class SingleInstance
    {
#if DEBUG
        public const string MutexName = "SBOutputController.SingleInstance.Debug";
        private const string ShowWindowMessageName = "SBOutputController.ShowWindow.Debug";
#else
        public const string MutexName = "SBOutputController.SingleInstance";
        private const string ShowWindowMessageName = "SBOutputController.ShowWindow";
#endif

        private static readonly IntPtr HWND_BROADCAST = new IntPtr(0xFFFF);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern uint RegisterWindowMessage(string message);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern bool PostMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// Windows hands out the same id to every instance that registers this name,
        /// which makes it usable as a private message between two instances of the application.
        /// </summary>
        public static readonly uint ShowWindowMessage = RegisterWindowMessage(ShowWindowMessageName);

        /// <summary>
        /// Asks the instance that is already running to show its window. The message is broadcast
        /// because the running instance may have started hidden, there is no window to look for.
        /// </summary>
        public static void RequestShowWindow()
        {
            PostMessage(HWND_BROADCAST, ShowWindowMessage, IntPtr.Zero, IntPtr.Zero);
        }
    }
}
