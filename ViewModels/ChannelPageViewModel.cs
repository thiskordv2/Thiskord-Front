using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Thiskord_Front.Models.Project;
using Thiskord_Front.Services;

namespace Thiskord_Front.ViewModels
{
    public partial class ChannelPageViewModel : ObservableObject
    {
        private readonly ChatService _chatService = ChatService.Instance;

        [ObservableProperty]
        private Message? _contextMessage;

        [ObservableProperty]
        private string messageInput = string.Empty ;
        public ObservableCollection<Message> Messages { get; } = new();
        private bool _isEditing = false;
        public async Task InitializeAsync(Channel channel)
        {
            Messages.Clear();

            _chatService.OnMessageReceived += OnMessageReceived;
            _chatService.OnMessageDeleted += OnMessageDeleted;
            _chatService.OnMessageEdited += OnMessageEdited;
            try
            {
                await _chatService.ConnectAsync();
                await _chatService.JoinChannelAsync(channel.Id.Value);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur connexion chat: " + ex.Message);
            }
        }

        public async Task CleanupAsync()
        {
            _chatService.OnMessageDeleted -= OnMessageDeleted;
            _chatService.OnMessageReceived -= OnMessageReceived;
            _chatService.OnMessageEdited -= OnMessageEdited;
            await _chatService.LeaveCurrentChannelAsync();
        }

        [RelayCommand]
        private void SetContextMessage(Message message) => ContextMessage = message;

        [RelayCommand]
        private void PrepareEditMessage(Message message)
        {
            ContextMessage = message;
            MessageInput = message.MsgText;
            _isEditing = true;
        }
        private bool CanSend() => !string.IsNullOrWhiteSpace(MessageInput);
        [RelayCommand(CanExecute = nameof(CanSend))]
        private async Task SendMessage()
        {
            if (_isEditing)
            {
                var editedMessage = MessageInput.Trim();
                await _chatService.EditMessage(ContextMessage!.Id, editedMessage);
                _isEditing = false;
                ContextMessage = null;
                MessageInput = string.Empty;
                return;
            }
            var message = MessageInput.Trim();
            MessageInput = string.Empty;
            try
            {
                await _chatService.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur envoi message: " + ex.Message);
            }
        }

        [RelayCommand]
        private async Task DeleteMessage(Message message)
        {
            if (message is null) return;
            try
            {
                await _chatService.DeleteMessage(message.Id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur suppression message: " + ex.Message);
            }
            finally
            {
                ContextMessage = null;
            }
        }

        public event Action<Action>? OnDispatchRequired;

        private void OnMessageReceived(Message message)
            => OnDispatchRequired?.Invoke(() => Messages.Add(message));

        private void OnMessageDeleted(int messageId)
            => OnDispatchRequired?.Invoke(() =>
            {
                var msg = Messages.FirstOrDefault(m => m.Id == messageId);
                if (msg is not null) Messages.Remove(msg);
            });

        private void OnMessageEdited(Message message)
            => OnDispatchRequired?.Invoke(() =>
            {
                var existing = Messages.FirstOrDefault(m => m.Id == message.Id);
                if (existing is not null)
                {
                    var index = Messages.IndexOf(existing);
                    if (index >= 0)
                    {
                        var updatedMessage = new Message
                        {
                            Id = existing.Id,
                            MsgAuthor = existing.MsgAuthor,
                            MsgDateTime = message.MsgDateTime,
                            MsgAlignment = existing.MsgAlignment,
                            MsgText = message.MsgText
                        };

                        Messages[index] = updatedMessage;
                    }
                }
            });

        partial void OnMessageInputChanged(string value)
            => SendMessageCommand.NotifyCanExecuteChanged();


    }
}
