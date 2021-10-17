using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using ContextMenu = System.Windows.Forms.ContextMenu;
using MenuItem = System.Windows.Forms.MenuItem;
using NotifyIcon = System.Windows.Forms.NotifyIcon;

namespace Umlaut
{
    public partial class MainWindow : Window
    {
        private NotifyIcon _systemTrayIcon;

        private event EventHandler _pauseMenuItemEventHandler;
        private event EventHandler _exitMenuItemEventHandler;

        private bool _inPauseState;

        private MenuItem _pauseMenuItem;
        private MenuItem _exitMenuItem;

        public MainWindow()
        {
            Hide();

            Stream onIcoStream = GetType().Assembly.GetManifestResourceStream("Umlaut.Resources.umlauton.ico");

            BitmapDecoder decoder = BitmapDecoder.Create(onIcoStream, BitmapCreateOptions.None, BitmapCacheOption.None);
            BitmapSource source = decoder.Frames[0];
            Icon = source;

            // Initialize system tray icon
            _systemTrayIcon = new NotifyIcon();
            _systemTrayIcon.Icon = new Icon(onIcoStream);
            _systemTrayIcon.Visible = true;
            //_systemTrayIcon.DoubleClick += OpenMenuItemClicked;

            _pauseMenuItemEventHandler += PauseMenuItemClicked;
            _exitMenuItemEventHandler += ExitMenuItemClicked;

            // Initialize menu items
            _pauseMenuItem = new MenuItem("Pause", _pauseMenuItemEventHandler);
            _exitMenuItem = new MenuItem("Exit", _exitMenuItemEventHandler);

            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(_pauseMenuItem);
            contextMenu.MenuItems.Add(_exitMenuItem);

            _systemTrayIcon.ContextMenu = contextMenu;

            InitializeComponent();

            HotKey hotKeyÄ = new HotKey(Key.A, KeyModifier.AltGr | KeyModifier.Shift, OnÄKeyHandler);
            HotKey hotKeyä = new HotKey(Key.A, KeyModifier.AltGr, OnäKeyHandler);
            HotKey hotKeyÖ = new HotKey(Key.O, KeyModifier.AltGr | KeyModifier.Shift, OnÖKeyHandler);
            HotKey hotKeyö = new HotKey(Key.O, KeyModifier.AltGr, OnöKeyHandler);
            HotKey hotKeyÜ = new HotKey(Key.U, KeyModifier.AltGr | KeyModifier.Shift, OnÜKeyHandler);
            HotKey hotKeyü = new HotKey(Key.U, KeyModifier.AltGr, OnüKeyHandler);
            HotKey hotKeyß = new HotKey(Key.S, KeyModifier.AltGr, OnßKeyHandler);

            _inPauseState = false;            
        }

        private void OnÄKeyHandler(HotKey hotKey) => SetUmlaut(Umlaute.Ä);
        private void OnäKeyHandler(HotKey hotKey) => SetUmlaut(Umlaute.ä);
        private void OnÖKeyHandler(HotKey hotKey) => SetUmlaut(Umlaute.Ö);
        private void OnöKeyHandler(HotKey hotKey) => SetUmlaut(Umlaute.ö);
        private void OnÜKeyHandler(HotKey hotKey) => SetUmlaut(Umlaute.Ü);
        private void OnüKeyHandler(HotKey hotKey) => SetUmlaut(Umlaute.ü);
        private void OnßKeyHandler(HotKey hotKey) => SetUmlaut(Umlaute.ß);
        private void SetUmlaut(string umlaut)
        {
            if (_inPauseState) return;

            _inPauseState = true;
            Clipboard.SetText(umlaut);
            Thread.Sleep(500);
            System.Windows.Forms.SendKeys.SendWait("^v");
            _inPauseState = false;
        }

        private void OpenMenuItemClicked(object sender, EventArgs e) =>  Show();

        private void PauseMenuItemClicked(object sender, EventArgs e) => TogglePause();

        private void ExitMenuItemClicked(object sender, EventArgs e) => Exit();

        private void TogglePause()
        {
            _inPauseState = !_inPauseState;
            _systemTrayIcon.Icon = new Icon(_inPauseState 
                ? GetType().Assembly.GetManifestResourceStream("Umlaut.Resources.umlautoff.ico")
                : GetType().Assembly.GetManifestResourceStream("Umlaut.Resources.umlauton.ico")
                );

            _pauseMenuItem.Text = _inPauseState ? "Run" : "Pause";
        }

        private void Exit()
        {
            _systemTrayIcon.Visible = false;
            Application.Current.Shutdown();
        }
    }
}
