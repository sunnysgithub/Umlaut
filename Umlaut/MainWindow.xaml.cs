using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using NotifyIcon = System.Windows.Forms.NotifyIcon;

namespace Umlaut
{
    public partial class MainWindow : Window
    {
        private HotKey _hotKey;
        private NotifyIcon _systemTrayIcon;

        public MainWindow()
        {
            InitializeComponent();

            Stream icoStream = GetType().Assembly.GetManifestResourceStream("Umlaut.Resources.umlaut.ico");
            BitmapDecoder decoder = IconBitmapDecoder.Create(icoStream, BitmapCreateOptions.None, BitmapCacheOption.None);
            BitmapSource source = decoder.Frames[0];
            Icon = source;

            _hotKey = new HotKey(Key.A, KeyModifier.Shift | KeyModifier.Ctrl, OnHotKeyHandler);

            _systemTrayIcon = new NotifyIcon();
            _systemTrayIcon.Icon = new Icon(icoStream);
            _systemTrayIcon.Visible = true;
            _systemTrayIcon.DoubleClick +=
            delegate (object sender, EventArgs args)
            {
                Show();
                WindowState = WindowState.Normal;
            };
        }

        private void OnHotKeyHandler(HotKey hotKey)
        {
            if (WindowState != WindowState.Normal)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState != WindowState.Normal)
            {
                Hide();
            }

            base.OnStateChanged(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;

            Hide();

            base.OnClosing(e);
        }

        void OnÄClicked(object sender, RoutedEventArgs e) => SetUmlaut(Umlaute.Ä);
        void OnäClicked(object sender, RoutedEventArgs e) => SetUmlaut(Umlaute.ä);
        void OnÖClicked(object sender, RoutedEventArgs e) => SetUmlaut(Umlaute.Ö);
        void OnöClicked(object sender, RoutedEventArgs e) => SetUmlaut(Umlaute.ö);
        void OnÜClicked(object sender, RoutedEventArgs e) => SetUmlaut(Umlaute.Ü);
        void OnüClicked(object sender, RoutedEventArgs e) => SetUmlaut(Umlaute.ü);
        void OnßClicked(object sender, RoutedEventArgs e) => SetUmlaut(Umlaute.ß);
        void OnExitClicked(object sender, RoutedEventArgs e)
        {
            _systemTrayIcon.Visible = false;
            Application.Current.Shutdown();
        }

        private void SetUmlaut(Umlaute umlaut)
        {
            switch (umlaut)
            {
                case Umlaute.Ä:
                    Clipboard.SetText("Ä");
                    break;
                case Umlaute.ä:
                    Clipboard.SetText("ä");
                    break;
                case Umlaute.Ö:
                    Clipboard.SetText("Ö");
                    break;
                case Umlaute.ö:
                    Clipboard.SetText("ö");
                    break;
                case Umlaute.Ü:
                    Clipboard.SetText("Ü");
                    break;
                case Umlaute.ü:
                    Clipboard.SetText("ü");
                    break;
                case Umlaute.ß:
                    Clipboard.SetText("ß");
                    break;
                default:
                    break;
            }
            Hide();
            System.Windows.Forms.SendKeys.SendWait("^v");
        }

        private void CancelCommand_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Hide();
        }

        private void CancelCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }
    }

    public enum Umlaute
    {
        Ä,
        ä,
        Ö,
        ö,
        Ü,
        ü,
        ß
    }
}
