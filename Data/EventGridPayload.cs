using System;

namespace SVV.MessagingApp.Data
{
    public class EventGridPayload<T> where T : class
    {
        public string ID { get; set; }
        
        public string Topic { get; set; }

        public string Subject { get; set; }
        
        public T Data { get; set; }
        
        public string EventType { get; set; }
        
        public string DataVersion { get; set; }
        
        public string MetadataVersion { get; set; }
        
        public DateTime EventTime { get; set; }
    }
}