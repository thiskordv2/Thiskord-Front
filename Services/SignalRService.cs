using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thiskord_Front.Services
{
    public class ChatService
    {    
        private HubConnection _hubConnection;

        public event Action<string, string> OnMessageReceived;

        public async Task ConnectAsync(string token)
        {
            _hubConnection = new HubConnectionBuilder()
                .WithUrl("[https://votre-api.com/chatHub](https://votre-api.com/chatHub)", options =>
                {
                    // Envoi du Token JWT pour l'authentification
                    options.AccessTokenProvider = () => Task.FromResult(token);
                })
                .WithAutomaticReconnect() // Reconnexion auto si coupure internet
                .Build();

            // Écouter les événements du serveur (Broadcast)
            _hubConnection.On<string, string>("ReceiveMessage", (user, message) =>
            {
                // Déclencher un événement pour que le ViewModel mette à jour l'UI
                OnMessageReceived?.Invoke(user, message);
            });

            await _hubConnection.StartAsync();
        }

        // Envoyer un message au serveur
        public async Task SendMessageAsync(string user, string message)
        {
            await _hubConnection.InvokeAsync("SendMessage", user, message);
        }
    }
}
