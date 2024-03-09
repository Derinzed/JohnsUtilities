using System;
using System.Collections.Generic;
using System.Text;

namespace JohnUtilities.Model.Classes
{
    public class NotificationEventArgs : EventArgs
    {
        public NotificationEventArgs(string message)
        {
            Message = message;
        }
        public string Message { get; protected set; }
    }
}
