using Windows.System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Thiskord_Front.ViewModels;

namespace Thiskord_Front.Views;

public sealed partial class Login : Page
{
    public LoginViewModel ViewModel { get; }

    public Login()
    {
        this.InitializeComponent();
        ViewModel = new LoginViewModel();
        ViewModel.OnLoginSuccess += () => this.Frame.Navigate(typeof(Navigateur));
        ViewModel.OnNavigateToRegister += () => this.Frame.Navigate(typeof(InscriptionPage));
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox pb)
            ViewModel.Password = pb.Password;
    }
    
    private void TextBox_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
    {
        if (e.Key == VirtualKey.Enter)
        {
            ViewModel.LoginCommand.Execute(null);
            e.Handled = true;
        }
    }
}