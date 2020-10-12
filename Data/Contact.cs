using System;

namespace SVV.MessagingApp.Data
{
    public class Contact
    {
        private string _displayName;
        public string DisplayName 
        { 
            get { return (String.IsNullOrEmpty(_displayName)) ? PrimaryNumber : _displayName; }
            set { _displayName = value; }
        }

        public string PrimaryNumber { get; set; }
    }
}