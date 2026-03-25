using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Thiskord_Front.Models.Project;
using Thiskord_Front.Services;

namespace Thiskord_Front.ViewModels
{
    public partial class ChannelPageViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<Message> messages  = new ();

        private readonly ChatService _chatService = ChatService.Instance;
        private readonly SessionService _sessionService;
        private Message? _contextMessageMenu;

        public ChannelPageViewModel()
        {
            _sessionService = SessionService.Instance;
        }

        [RelayCommand]
        private async Task SendMessage(string message)
        {
            try
            {
                await _chatService.SendMessageAsync(message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Erreur envoi message: " + ex.Message);
            }
        }


    }
}
