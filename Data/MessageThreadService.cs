using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Azure.Communication;
using Azure.Communication.Sms;
using Microsoft.EntityFrameworkCore;

namespace SVV.MessagingApp.Data
{
    public class MessageThreadService
    {
        private static List<MessageThread> messageThreads;       
        
        public Task<List<MessageThread>> GetMessageThreadsAsync(string primaryPhoneNumber, string dbConnectionString)
        {
            using (var db = new MessageThreadDbContext(dbConnectionString))
            {
                messageThreads = db.MessageThreads.ToList<MessageThread>();
                foreach (var thread in messageThreads)
                {
                    db.Entry(thread)
                        .Collection(t => t.Messages)
                        .Load();
                }
            }

            return Task.FromResult<List<MessageThread>>(messageThreads);
        }

        public Task SaveMessageThreadAsync(MessageThread messageThread, bool saveMessages, string dbConnectionString)
        { 
            using (var db = new MessageThreadDbContext(dbConnectionString))
            {
                db.Entry(messageThread).State = EntityState.Modified;
                db.MessageThreads.Update(messageThread);

                if (saveMessages)
                {
                    // TODO: Cascade save of messages
                }

                db.SaveChanges();
            }

            return Task.FromResult<bool>(true);
        }

        public void OnSmsEventReceived(SmsEvent smsEvent, string dbConnectionString) 
        {
            // Find message thread
            var thread = messageThreads.Find((t) => t.SecondaryPhoneNumber == smsEvent.From);
            if (thread == null)
            {
                thread = new MessageThread() 
                {
                    ID = new Guid(),
                    PrimaryPhoneNumber = smsEvent.To,
                    SecondaryPhoneNumber = smsEvent.From,
                    Messages = new List<Message>()
                };
                messageThreads.Add(thread);
            }
            
            var newMessage = new Message()
            {
                ID = smsEvent.MessageId,
                From = smsEvent.From,
                To = smsEvent.To,
                Date = smsEvent.ReceivedTimestamp,
                Body = smsEvent.Message,
                Read = false
            };
            thread.Messages.Add(newMessage);
            thread.Read = false;

            using (var db = new MessageThreadDbContext(dbConnectionString))
            {
                db.Entry(thread).State = EntityState.Modified;
                db.MessageThreads.Update(thread);

                db.Entry(newMessage).State = EntityState.Added;
                db.Messages.Add(newMessage);

                db.SaveChanges();
            }
        }

        public Task<string> SendNewMessageAsync(MessageThread thread, Message message, string acsConnectionString, string dbConnectionString)
        {
            SmsClient smsClient;
            string response = String.Format("Message to {0} send at {1}", message.To, message.Date);

            if (String.IsNullOrEmpty(acsConnectionString)) {
                throw new NullReferenceException("The connection string to Azure Communication Services is not set.");
            }

            try 
            {
                // TODO: Address formating of numbers correctly. Numbers require the + in the SmsClient API, but are
                // received in the EventGrid payload without the + so makes matching difficult.

                smsClient = new SmsClient(acsConnectionString);
                var smsResult = smsClient.Send(
                    from : new PhoneNumber(String.Format("+{0}", message.From)),
                    to : new PhoneNumber(String.Format("+{0}", message.To)),
                    message : message.Body
                );

                message.ID = smsResult.Value.MessageId;
                thread.Messages.Add(message);

                response = String.Format("{0} with ID {1}", response, message.ID);

                using (var db = new MessageThreadDbContext(dbConnectionString))
                {
                    db.Entry(thread).State = EntityState.Modified;
                    db.MessageThreads.Update(thread);

                    db.Entry(message).State = EntityState.Added;
                    db.Messages.Add(message);

                    db.SaveChanges();
                }

            }
            catch (Exception e)
            {
                response = String.Format("Unable to send SMS message: {0}", e.Message);
            }
            finally
            {
                smsClient = null;
            }

            return Task.FromResult<string>(response);
        }
    }
}