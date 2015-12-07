using MessageQueuing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueuing
{
    public delegate void MessageReceivedEventHandler<T>(Object sender, MessageReceivedEventArgs<T> e);
    public class MessageQueueExtended<T> : MessageQueue where T : class, new()
    {
        public event MessageReceivedEventHandler<T> MessageReceived;
        public MessageQueueExtended(String path) : base(path)
        {
            this.Formatter = new JsonMessageFormatter<T>(Encoding.UTF8);
            this.ReceiveCompleted += MessageQueueExtended_ReceiveCompleted;
            this.BeginReceive();
        }

        private void MessageQueueExtended_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            try
            {
                if (e.Message != null && e.Message.Body != null)
                {
                    var message = e.Message.Body as T;
                    if (message != null)
                    {
                        OnMessageReceived(new MessageReceivedEventArgs<T>(message));
                    }
                }

                this.BeginReceive();
            }
            catch (MessageQueueException)
            {
                //Log exception
            }
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs<T> e)
        {
            if (MessageReceived != null)
            {
                MessageReceived(this, e);
            }
        }

        public void SendMessage(T message)
        {
            this.Send(message);
        }

        public T ReceiveMessage()
        {
            return this.Receive() as T;
        }
    }
}
