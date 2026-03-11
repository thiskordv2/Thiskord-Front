using Microsoft.UI;
using Microsoft.UI.Composition;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using Windows.Graphics;
using Windows.UI;
using WinRT;

namespace Thiskord_Front
{
    public sealed partial class MainWindow : Window
    {
        private DesktopAcrylicController _acrylicController;
        private SystemBackdropConfiguration _backdropConfig;

        public MainWindow()
        {
            InitializeComponent();

            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(AppTitleBar);

            IntPtr hWnd = WinRT.Interop.WindowNative.GetWindowHandle(this);
            WindowId windowId = Microsoft.UI.Win32Interop.GetWindowIdFromWindow(hWnd);
            AppWindow appWindow = AppWindow.GetFromWindowId(windowId);
            appWindow.SetIcon("Assets/asterion-logo.ico");
            AppWindow.TitleBar.PreferredTheme = TitleBarTheme.UseDefaultAppMode;

            CenterWindow();
            SetupAcrylic();

            RootFrame.Navigate(typeof(Login));
        }

        private void SetupAcrylic()
        {
            if (!DesktopAcrylicController.IsSupported())
                return;

            _backdropConfig = new SystemBackdropConfiguration
            {
                IsInputActive = true,
                Theme = SystemBackdropTheme.Dark
            };

            _acrylicController = new DesktopAcrylicController
            {
                TintColor         = Color.FromArgb(255, 32, 32, 32),
                TintOpacity       = 0.75f,
                LuminosityOpacity = 0.85f,
                Kind              = DesktopAcrylicKind.Base
            };

            // ICompositionSupportsSystemBackdrop est dans Microsoft.UI.Composition
            _acrylicController.AddSystemBackdropTarget(
                this.As<ICompositionSupportsSystemBackdrop>()
            );
            _acrylicController.SetSystemBackdropConfiguration(_backdropConfig);

            this.Activated += (s, e) =>
                _backdropConfig.IsInputActive =
                    e.WindowActivationState != WindowActivationState.Deactivated;

            this.Closed += (s, e) =>
            {
                _acrylicController.Dispose();
                _acrylicController = null;
            };
        }

        private void CenterWindow()
        {
            var area = DisplayArea.GetFromWindowId(AppWindow.Id, DisplayAreaFallback.Nearest)?.WorkArea;
            if (area == null) return;
            AppWindow.Move(new PointInt32(
                (area.Value.Width - AppWindow.Size.Width) / 2,
                (area.Value.Height - AppWindow.Size.Height) / 2
            ));
        }
    }
}
