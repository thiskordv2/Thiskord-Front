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
                    options.AccessTokenProvider = () => Task.FromResult(_sessionService.Token);
                    // options.Headers["username"] = _sessionService.CurrentUser;
                })
                .WithAutomaticReconnect()
                .Build();

            _hubConnection.On<List<MessageDto>>("LoadMessages", (messages) =>
            {
               
                foreach (var m in messages)
                {
                    var alignement = m.User == _sessionService.CurrentUser
                    ? Microsoft.UI.Xaml.HorizontalAlignment.Right
                    : Microsoft.UI.Xaml.HorizontalAlignment.Left;
                    OnMessageReceived?.Invoke(new Message
                    {
                        MsgText = $"{m.User} : {m.Text}",
                        MsgDateTime = m.DateTime,
                        MsgAlignment = alignement
                    });
                }
            });

            _hubConnection.On<string, string, string>("ReceiveMessage", (user, text, dateTime) =>
            {
                var alignement = user == _sessionService.CurrentUser
                    ? Microsoft.UI.Xaml.HorizontalAlignment.Right
                    : Microsoft.UI.Xaml.HorizontalAlignment.Left; 
                var message = new Message
                {
                    MsgText = $"{user} : {text}",
                    MsgDateTime = dateTime,
                    MsgAlignment = alignement
                };
                OnMessageReceived?.Invoke(message);
            });

            _hubConnection.On<string, string>("UserJoined", (user, text) =>
            {
                var message = new Message
                {
                    MsgText = $"{user} : {text}",
                    MsgAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Left
                };
                OnMessageReceived?.Invoke(message);
            });

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
    }
}
