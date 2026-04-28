using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Thiskord_Front.Models.Project
{
    public partial class Message : ObservableObject
    {
        public int Id { get; set; }

        [ObservableProperty]
        public string _msgText = string.Empty;

        [ObservableProperty]
        public string _msgDateTime = string.Empty;
        public string MsgAuthor { get; set; }
        public HorizontalAlignment MsgAlignment { get; set; }
    }
}
