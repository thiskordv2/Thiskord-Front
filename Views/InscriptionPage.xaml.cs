using ABI.Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Thiskord_Front.ViewModels;
using Thiskord_Front.Services;
using Windows.UI;

namespace Thiskord_Front.Views
{
    public sealed partial class InscriptionPage : Page
    {
        private readonly AuthService authService = new();

        public InscriptionViewModel ViewModel { get; }

        public InscriptionPage()
        {
            this.InitializeComponent();
            ViewModel = new InscriptionViewModel();
            ViewModel.OnRegisterSuccess += () => this.Frame.Navigate(typeof(Login));
            ViewModel.OnNavigateToLogin += () => this.Frame.Navigate(typeof(Login));
        }

        private void PasswordBox1_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb)
                ViewModel.Password1 = pb.Password;
        }

        private void PasswordBox2_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox pb)
                ViewModel.Password2 = pb.Password;
        }
    }
}
