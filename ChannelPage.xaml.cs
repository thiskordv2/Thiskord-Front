using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Thiskord_Front.Models.Project;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Thiskord_Front
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ChannelPage : Page
    {
        public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>();

        public ChannelPage()
        {
            InitializeComponent();
            InvertedListView.ItemsSource = Messages;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (e.Parameter is Channel channel)
            {
                Messages.Clear();
                Messages.Add(new Message { MsgText = $"Bienvenue dans #{channel.Name}", MsgDateTime = DateTime.Now.ToString("HH:mm"), MsgAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Left });
                Messages.Add(new Message { MsgText = "Message de test", MsgDateTime = DateTime.Now.ToString("HH:mm"), MsgAlignment = Microsoft.UI.Xaml.HorizontalAlignment.Right });
            }
        }
    }
}