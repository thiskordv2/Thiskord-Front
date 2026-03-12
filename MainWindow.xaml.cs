using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics;
using Thiskord_Front.Pages;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Thiskord_Front
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // R�cup�rer l'identifiant de la fen�tre (Handle)
            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);

            // D�finir l'ic�ne (chemin vers votre fichier .ico dans Assets)
            appWindow.SetIcon("Assets/asterion-logo.ico");

            AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;
            // Center the window on the screen.
            CenterWindow();


            this.Content = new Login();
        }

        // Centers the given AppWindow on the screen based on the available display area.
        private void CenterWindow()
        {
            var area = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Nearest)?.WorkArea;
            if (area == null) return;
            AppWindow.Move(new PointInt32((area.Value.Width - AppWindow.Size.Width) / 2, (area.Value.Height - AppWindow.Size.Height) / 2));
        }

    }
}
