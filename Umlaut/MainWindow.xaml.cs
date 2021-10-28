using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
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
        private const string REGISTRY_PATH = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
        private const string REGISTRY_KEY = "Umlaut";

        private NotifyIcon _systemTrayIcon;

        private event EventHandler _autostartMenuItemEventHandler;
        private event EventHandler _pauseMenuItemEventHandler;
        private event EventHandler _exitMenuItemEventHandler;

        private bool _isAutostartActivated;
        private bool _inPauseState;

        private MenuItem _autostartMenuItem;
        private MenuItem _pauseMenuItem;
        private MenuItem _exitMenuItem;

        private IEnumerable<HotKey> _hotkeys;

        public MainWindow()
        {
            _isAutostartActivated = IsAutostartActivated();

            _hotkeys = new List<HotKey>()
            {
                new HotKey(Key.A, KeyModifier.AltGr | KeyModifier.Shift, () => SetUmlaut(Umlaute.Ä)),
                new HotKey(Key.A, KeyModifier.AltGr, () => SetUmlaut(Umlaute.ä)),
                new HotKey(Key.O, KeyModifier.AltGr | KeyModifier.Shift, () => SetUmlaut(Umlaute.Ö)),
                new HotKey(Key.O, KeyModifier.AltGr, () => SetUmlaut(Umlaute.ö)),
                new HotKey(Key.U, KeyModifier.AltGr | KeyModifier.Shift, () => SetUmlaut(Umlaute.Ü)),
                new HotKey(Key.U, KeyModifier.AltGr, () => SetUmlaut(Umlaute.ü)),
                new HotKey(Key.S, KeyModifier.AltGr, () => SetUmlaut(Umlaute.ß))
            };

            Stream onIcoStream = GetType().Assembly.GetManifestResourceStream("Umlaut.Resources.umlauton.ico");

            BitmapDecoder decoder = BitmapDecoder.Create(onIcoStream, BitmapCreateOptions.None, BitmapCacheOption.None);
            BitmapSource source = decoder.Frames[0];
            Icon = source;

            // Initialize system tray icon
            _systemTrayIcon = new NotifyIcon();
            _systemTrayIcon.Icon = new Icon(onIcoStream);
            _systemTrayIcon.Visible = true;

            _autostartMenuItemEventHandler += AutostartMenuItemClicked;
            _pauseMenuItemEventHandler += PauseMenuItemClicked;
            _exitMenuItemEventHandler += ExitMenuItemClicked;

            // Initialize menu items
            _autostartMenuItem = new MenuItem(_isAutostartActivated ? "Deactivate Autostart" : "Activate Autostart", _autostartMenuItemEventHandler);
            _pauseMenuItem = new MenuItem("Pause", _pauseMenuItemEventHandler);
            _exitMenuItem = new MenuItem("Exit", _exitMenuItemEventHandler);

            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add(_autostartMenuItem);
            contextMenu.MenuItems.Add(_pauseMenuItem);
            contextMenu.MenuItems.Add(_exitMenuItem);

            _systemTrayIcon.ContextMenu = contextMenu;

            InitializeComponent();

            _inPauseState = false;            
        }

        private void SetUmlaut(char umlaut)
        {
            if (_inPauseState) return;

            InputSimulator.SendInput(umlaut);
        }

        private void AutostartMenuItemClicked(object sender, EventArgs e) => ToggleAutostart();

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

        private void ToggleAutostart()
        {
            if(_isAutostartActivated)
            {
                RemoveUmlautFromStartup();
            }
            else
            {
                LaunchUmlautAtStartup();
            }
        }

        private bool IsAutostartActivated()
        {
            try
            {
                RegistryKey regKey = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH);
                return regKey.GetValueNames().Contains(REGISTRY_KEY);
            }
            catch(Exception)
            {
                return false;
            }
        }

        private void LaunchUmlautAtStartup()
        {
            try
            {
                string appPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH, true))
                {
                    if (regKey is null) return;
                    regKey.SetValue(REGISTRY_KEY, Path.Combine(appPath, "Umlaut.exe"));
                    _autostartMenuItem.Text = "Deactivate Autostart";
                    _isAutostartActivated = true;
                }
            }
            catch(Exception) {}
        }

        private void RemoveUmlautFromStartup()
        {
            try
            {
                using (RegistryKey regKey = Registry.CurrentUser.OpenSubKey(REGISTRY_PATH, true))
                {
                    if (regKey is null) return;
                    regKey.DeleteValue(REGISTRY_KEY);
                    _autostartMenuItem.Text = "Activate Autostart";
                    _isAutostartActivated = false;
                }
            }
            catch (Exception) { }
        }

        private void Exit()
        {
            foreach(HotKey hotkey in _hotkeys)
            {
                hotkey.Unregister();
            }

            _systemTrayIcon.Visible = false;
            Application.Current.Shutdown();
        }
    }    
}
