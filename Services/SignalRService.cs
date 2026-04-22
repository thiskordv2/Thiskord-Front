using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Thiskord_Front.Models.Project;
using static System.Net.Mime.MediaTypeNames;

namespace Thiskord_Front.Services
{
    public class ChatService
    {
        private static readonly Lazy<ChatService> _instance = new(() => new ChatService());
        public static ChatService Instance => _instance.Value;

        private HubConnection? _hubConnection;
        private int? _currentChannelId;

        public event Action<Message>? OnMessageReceived;
        public event Action<int>? OnMessageDeleted;
        public event Action<Message>? OnMessageEdited;
        public readonly SessionService _sessionService;

        private ChatService()
        {
            _sessionService = SessionService.Instance;
        }

        public bool IsConnected => _hubConnection?.State == HubConnectionState.Connected;

        public async Task ConnectAsync()
        {
            if (IsConnected) return;

            _hubConnection = new HubConnectionBuilder()
                .WithUrl("http://localhost:8080/chatHub", options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult<string?>(_sessionService.Token);
                })
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<List<MessageDto>>("LoadMessages", (messages) =>
            {

                foreach (var m in messages)
                {
                    OnMessageReceived?.Invoke(new Message
                    {
                        Id = m.Id,
                        MsgAuthor = m.Username,
                        MsgText = $"{m.Content}",
                        MsgDateTime = m.CreatedAt,
                        MsgAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Left
                    });
                }
            });

            _hubConnection.On<int, string, string, string>("ReceiveMessage", (id, user, text, dateTime) =>
            {
                var message = new Message
                {
                    Id=id,
                    MsgAuthor = user,
                    MsgText = $"{text}",
                    MsgDateTime = dateTime,
                    MsgAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Left
                };
                OnMessageReceived?.Invoke(message);
            });

            _hubConnection.On<int>("DeleteMessage", (messageId) =>
            {
                OnMessageDeleted?.Invoke(messageId);
            });
            _hubConnection.On<int, string>("EditMessage", (messageId, newText) =>
            {
                var editedMessage = new Message
                {
                    Id = messageId,
                    MsgText = newText,
                    MsgAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Left
                };
                OnMessageEdited?.Invoke(editedMessage);
            }); ;

            await _hubConnection.StartAsync();
        }

        public async Task JoinChannelAsync(int channelId)
        {
            if (!IsConnected) return;

            if (_currentChannelId.HasValue)
                await _hubConnection!.InvokeAsync("LeaveChannel", _currentChannelId.Value);

            await _hubConnection!.InvokeAsync("JoinChannel", channelId);
            _currentChannelId = channelId;
        }

        public async Task LeaveCurrentChannelAsync()
        {
            if (!IsConnected || !_currentChannelId.HasValue) return;

            await _hubConnection!.InvokeAsync("LeaveChannel", _currentChannelId.Value);
            _currentChannelId = null;
        }

        public async Task SendMessageAsync(string text)
        {
            if (!IsConnected || !_currentChannelId.HasValue) return;

            await _hubConnection!.InvokeAsync("SendMessage", _currentChannelId.Value, text);
        }

        public async Task DisconnectAsync()
        {
            if (_hubConnection is not null)
                await _hubConnection.StopAsync();
        }

        public async Task DeleteMessage(int messageId)
        {
            if (!IsConnected || !_currentChannelId.HasValue) return;
            await _hubConnection!.InvokeAsync("DeleteMessage", _currentChannelId.Value, messageId);
        }

        public async Task EditMessage(int messageId, string newText)
        {
            if (!IsConnected || !_currentChannelId.HasValue) return;
            await _hubConnection!.InvokeAsync("EditMessage", _currentChannelId.Value, messageId, newText);
            // FIX: supprimé l'appel manuel à OnMessageEdited — le hub le déclenche pour tous les clients
        }
    }
}
