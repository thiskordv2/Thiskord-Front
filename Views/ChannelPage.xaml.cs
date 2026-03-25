using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Thiskord_Front.Models.Project;
using Thiskord_Front.Services;
using Thiskord_Front.ViewModels;
using Windows.System;

namespace Thiskord_Front.Views
{
    public sealed partial class ChannelPage : Page
    {
        public ChannelPageViewModel ViewModel { get; }

        public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>();

        private readonly ChatService _chatService = ChatService.Instance;
        private readonly SessionService _sessionService;
        private Message? _contextMessageMenu;

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
            _chatService.OnMessageDeleted += OnMessageDeleted;



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
            _chatService.OnMessageDeleted -= OnMessageDeleted;
            await _chatService.LeaveCurrentChannelAsync();
        }

        private void OnMessageReceived(Message message)
        {
            DispatcherQueue.TryEnqueue(() => Messages.Add(message));
        }

        private void OnMessageDeleted(int messageId) 
        {
            DispatcherQueue.TryEnqueue(() =>
            {
                var msgToRemove = Messages.FirstOrDefault(m => m.Id == messageId);
                if (msgToRemove != null)
                {
                    Messages.Remove(msgToRemove);
                }
            });
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
        private void Message_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (sender is not FrameworkElement element) return;

            _contextMessageMenu = element.DataContext as Message;

            if (_contextMessageMenu is null) return;

            // Afficher le menu
            if (sender is TextBlock tb)
            {
                var menu = new MenuFlyout();
                var deleteItem = new MenuFlyoutItem { Text = "Supprimer", Tag = "delete" };
                deleteItem.Click += MessageContextMenu_Click;
                menu.Items.Add(deleteItem);
                menu.ShowAt(tb, e.GetPosition(tb));
            }
        }

        private async void MessageContextMenu_Click(object sender, RoutedEventArgs e)
        {
            if (_contextMessageMenu is null || sender is not MenuFlyoutItem item)
                return;

            switch (item.Tag as string)
            {
                case "edit":
                    //await EditChannelAsync(_contextMessageMenu);
                    break;

                case "delete":
                    await _chatService.DeleteMessage(_contextMessageMenu.Id);
                    _contextMessageMenu = null;
                    break;
            }

            _contextMessageMenu = null;
        }

    }
}