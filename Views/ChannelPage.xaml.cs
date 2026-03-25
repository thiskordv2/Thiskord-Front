using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Navigation;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Thiskord_Front.Models.Project;
using Thiskord_Front.Services;
using Windows.System;

namespace Thiskord_Front.Views
{
    public sealed partial class ChannelPage : Page
    {
        public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>();

        private readonly ChatService _chatService = ChatService.Instance;
        private readonly SessionService _sessionService;

        public ChannelPage()
        {
            InitializeComponent();
            InvertedListView.ItemsSource = Messages;
            _sessionService = SessionService.Instance;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is not Channel channel) return;

            Messages.Clear();

            _chatService.OnMessageReceived += OnMessageReceived;

            try
            {
                await _chatService.ConnectAsync();
                await _chatService.JoinChannelAsync(channel.Id!.Value);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur envoi message: " + ex.Message);
            }
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            _chatService.OnMessageReceived -= OnMessageReceived;
            await _chatService.LeaveCurrentChannelAsync();
        }

        private void OnMessageReceived(Message message)
        {
            DispatcherQueue.TryEnqueue(() => Messages.Add(message));
        }

        private async void OnSendButton_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(MessageInput.Text))
            {
                string msg = MessageInput.Text;
                try
                {
                    await _chatService.SendMessageAsync(msg);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Erreur envoi message: " + ex.Message);
                }
                MessageInput.Text = string.Empty;
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("Erreur, message vide");
            }
        }

        private void TextBox_PreviewKeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                OnSendButton_Click(sender, e);
                e.Handled = true;
            }
        }
    }
}