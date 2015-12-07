using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Configuration;

namespace MessageQueuing
{
    public delegate void MessageReceivedEventHandler<T>(Object sender, MessageReceivedEventArgs<T> e);

    public class MessageQueueManager<T> : IDisposable where T : class, new()
    {

        #region Fields
        private string queueName;
        private MessageQueue messageQueue;
        private bool disposing = false;
        private IMessageFormatter messageFormatter = new JsonMessageFormatter<T>(Encoding.UTF8);
        private bool raiseEvents = false;
        IAsyncResult asyncResult = null;
        #endregion

        #region Constants

        #endregion

        #region Events
        /// <summary>
        /// Event raised when the message of type T is fetched from the message queue
        /// </summary>
        public event MessageReceivedEventHandler<T> MessageReceived;
        #endregion

        #region Properties
        /// <summary>
        /// Private queue path
        /// </summary>
        public string MessageQueueName
        {
            get
            {
                return this.queueName;
            }
        }


        public bool RaiseEvents
        {
            get
            {
                return this.raiseEvents;
            }
            set
            {
                if (value)
                {
                    asyncResult = this.messageQueue.BeginReceive();
                }
                else
                {
                    if (this.asyncResult != null)
                    {
                        this.messageQueue.EndReceive(this.asyncResult);
                        this.asyncResult = null;
                    }
                }
                this.raiseEvents = value;
            }
        }

        /// <summary>
        /// The MessageQueue instance which is wrapped in MessegeQueueManager instance
        /// </summary>
        public MessageQueue MessageQueue
        {
            get
            {
                return this.messageQueue;
            }
        }


        /// <summary>
        /// Message formatter for serializing the queue messages
        /// </summary>
        public IMessageFormatter MessageFormatter
        {
            get
            {
                return this.messageFormatter;
            }
        }

        #endregion

        #region Constructors

        public MessageQueueManager(string queueName, IMessageFormatter messageFormatter)
        {
            this.queueName = queueName;
            this.messageFormatter = messageFormatter;
            this.InitiateMessageQueueManager();
        }

        public MessageQueueManager(string queueName)
        {
            this.queueName = queueName;
            this.InitiateMessageQueueManager();
        }
        #endregion

        #region Methods

        private void InitiateMessageQueueManager()
        {
            this.messageQueue = new MessageQueue(queueName);
            this.messageQueue.Formatter = this.messageFormatter;
            this.messageQueue.ReceiveCompleted += MessageQueue_ReceiveCompleted;
        }

        private void MessageQueue_ReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            if (e.Message != null && e.Message.Body != null)
            {
                var message = e.Message.Body as T;
                if (message != null)
                {
                    OnMessageReceived(new MessageReceivedEventArgs<T>(message));
                }
            }
        }


        /// <summary>
        /// Adds message to message queue
        /// </summary>
        /// <param name="queueMessageObject">Object to be put to message queue</param>
        public void AddMessage(T queueMessageObject)
        {
            messageQueue.Send(queueMessageObject);
        }

        /// <summary>
        /// Adds message to message queue
        /// </summary>
        /// <param name="queueMessageObject">Object to be put to message queue</param>
        /// <param name="messageExpiration">How long message will be kept in the queue until it expires</param>
        public void AddMessage(T queueMessageObject, int messageExpiration)
        {
            var message = new Message(queueMessageObject);
            message.TimeToBeReceived = TimeSpan.FromMinutes(messageExpiration);
            messageQueue.Send(message);
        }

        public T GetMessage()
        {
            return this.messageQueue.Receive() as T;
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs<T> e)
        {
            if (MessageReceived != null)
            {
                MessageReceived(this, e);
            }
        }

        /// <summary>
        /// Stops the message queue listener and disposes the object
        /// </summary>
        public void Dispose()
        {
            if (!disposing)
            {
                disposing = true;
                messageQueue.Dispose();
            }
        }

        #endregion

    }
}
