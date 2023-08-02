using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientAppTCP.ViewModels
{
    public class MessageUCViewModel : BaseViewModel
    {

        private string messageText;

        public string MessageText
        {
            get { return messageText; }
            set { messageText = value; OnPropertyChanged(); }
        }

    }
}
