using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SBOutputController
{
    public partial class HotKeyEditorControl
    {
        static readonly private Brush RegistrationFailedBackground = new SolidColorBrush(Color.FromRgb(189, 92, 92));

        public static readonly DependencyProperty HotKeyProperty =
            DependencyProperty.Register(nameof(HotKey), typeof(HotKey),
                typeof(HotKeyEditorControl),
                new FrameworkPropertyMetadata(default(HotKey),
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public HotKey HotKey
        {
            get => (HotKey)GetValue(HotKeyProperty);
            set
            {
                SetValue(HotKeyProperty, value);
                RaiseEvent(new RoutedEventArgs(HotKeyChangedEvent));
            }
        }

        public static readonly RoutedEvent HotKeyChangedEvent = EventManager.RegisterRoutedEvent("HotKeyChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(HotKeyEditorControl));

        public event RoutedEventHandler HotKeyChanged
        {
            add { AddHandler(HotKeyChangedEvent, value); }
            remove { RemoveHandler(HotKeyChangedEvent, value); }
        }

        public HotKeyEditorControl()
        {
            InitializeComponent();

            GotKeyboardFocus += HotKeyEditorControl_GotKeyboardFocus;
        }

        /// <summary>
        /// Marks the hot key as unusable, windows hands out a hot key to a single application only
        /// and single key hot keys are a lot more likely to be taken already.
        /// </summary>
        public void SetRegistrationFailed(bool failed)
        {
            if (failed)
            {
                HotKeyTextBox.Background = RegistrationFailedBackground;
                HotKeyTextBox.Foreground = Brushes.White;
                HotKeyTextBox.ToolTip = "Registering this hotkey failed, another application is already using it";
            }
            else
            {
                HotKeyTextBox.ClearValue(Control.BackgroundProperty);
                HotKeyTextBox.ClearValue(Control.ForegroundProperty);
                HotKeyTextBox.ClearValue(ToolTipProperty);
            }
        }

        private void HotKeyEditorControl_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            HotKeyTextBox.Text = "Recording...";
        }

        private void HotKeyTextBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            // Get modifiers and key data
            var modifiers = Keyboard.Modifiers;
            var key = e.Key;

            // When Alt is pressed, SystemKey is used instead
            if (key == Key.System)
            {
                key = e.SystemKey;
            }

            // Tab on its own is left alone so it can still move the focus out of the control
            if (key == Key.Tab && modifiers == ModifierKeys.None)
            {
                return;
            }

            // Don't let the event pass further
            // because we don't want standard textbox shortcuts working
            e.Handled = true;

            // Pressing delete, backspace or escape without modifiers clears the current value
            if (modifiers == ModifierKeys.None && (key == Key.Delete || key == Key.Back || key == Key.Escape))
            {
                HotKey = null;
                return;
            }

            // If no actual key was pressed - return
            // Keys without any modifiers are allowed, they end up as single key hotkeys
            if (key == Key.LeftCtrl ||
                key == Key.RightCtrl ||
                key == Key.LeftAlt ||
                key == Key.RightAlt ||
                key == Key.LeftShift ||
                key == Key.RightShift ||
                key == Key.LWin ||
                key == Key.RWin ||
                key == Key.Clear ||
                key == Key.OemClear ||
                key == Key.Apps)
            {
                return;
            }

            // Update the value
            HotKey = new HotKey(key, modifiers);
        }
    }
}
