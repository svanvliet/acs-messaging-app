using System;

namespace SVV.MessagingApp.Data
{
    public class Message
    {
        public string ID { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public DateTime Date { get; set; }

        public string Body { get; set; }

        public bool Read { get; set; }

        public bool Delivered { get; set; }
    }
}