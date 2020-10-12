using System;
using System.Collections.Generic;

namespace SVV.MessagingApp.Data
{
    public class MessageThread
    {
        public MessageThread()
        {
            Messages = new List<Message>();
        }

        public Guid ID { get; set; }

        public string PrimaryPhoneNumber { get; set; }

        public string SecondaryPhoneNumber { get; set; }

        public List<Message> Messages { get; set; }

        public bool Read { get; set; }
    }
}