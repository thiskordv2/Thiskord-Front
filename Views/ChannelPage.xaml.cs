using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using Thiskord_Front.Models.Project;
using Thiskord_Front.ViewModels;
using Windows.System;

namespace Thiskord_Front.Views
{
    public sealed partial class ChannelPage : Page
    {
        public ChannelPageViewModel ViewModel { get; } = new();

        public ChannelPage()
        {
            InitializeComponent();

            ViewModel.OnDispatchRequired += action => DispatcherQueue.TryEnqueue(action.Invoke);
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Channel channel) await ViewModel.InitializeAsync(channel); 
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            await ViewModel.CleanupAsync();
        }

        private void TextBox_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                ViewModel.SendMessageCommand.Execute(null);
                e.Handled = true;
            }
        }
        private void Message_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (sender is FrameworkElement { DataContext: Message message}) ViewModel.SetContextMessageCommand.Execute(message);
        }
    }
}