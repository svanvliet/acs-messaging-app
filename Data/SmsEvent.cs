using System;

namespace SVV.MessagingApp.Data
{
    public class SmsEvent
    {
        public string MessageId { get; set; }

        public string From { get; set; }

        public string To { get; set; }

        public string Message { get; set; }
        
        public DateTime ReceivedTimestamp { get; set; }
    }
}