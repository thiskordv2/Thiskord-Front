using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Thiskord_Front.ViewModels;
using Thiskord_Front.Pages;

namespace Thiskord_Front;

public sealed partial class Login : Page
{
    public LoginViewModel ViewModel { get; }

    public Login()
    {
        this.InitializeComponent();
        ViewModel = new LoginViewModel();

        ViewModel.OnLoginSuccess += () => this.Frame.Navigate(typeof(Navigateur));
        ViewModel.OnLoginFailed += (msg) => LoginErrorText.Text = msg;
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox pb)
            ViewModel.Password = pb.Password;
    }

    private void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        this.Frame.Navigate(typeof(InscriptionPage));
    }
}