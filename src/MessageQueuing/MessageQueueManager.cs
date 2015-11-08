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
        private bool readQueue = true;
        private bool disposing = false;
        private int? readTimeout = null;
        private CancellationTokenSource cancellationTokenSource;
        private CancellationToken cancellationToken;
        private IMessageFormatter messageFormatter = new JsonMessageFormatter<T>(Encoding.UTF8);
        #endregion

        #region Constants
        private const int MAXIMUMTHREADS = 10;
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

        /// <summary>
        /// Start rwading with event raising
        /// </summary>
        public bool RaiseEvents { get; set; }

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
        /// Maximum number of threads to run for rading the queue
        /// TODO: Implement multiple threads to read the queue
        /// </summary>
        private int MaxumumParallelReadings
        {
            get
            {
                int maxThreadNum;
                if (int.TryParse(ConfigurationManager.AppSettings.Get("MessageQueuing.MaximumThreads"), out maxThreadNum))
                {
                    return maxThreadNum;
                }
                return MAXIMUMTHREADS;
            }
        }

        /// <summary>
        /// Message Queue read timeout in miliseconds
        /// </summary>
        public int? ReadTimeout
        {
            get
            {
                return readTimeout;

            }
            set
            {
                readTimeout = value;
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

            this.cancellationTokenSource = new CancellationTokenSource();
            this.cancellationToken = cancellationTokenSource.Token;


            Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                while (readQueue)
                {
                    if (cancellationToken.IsCancellationRequested)
                    {
                        break;
                    }
                    else
                    {
                        if (MessageReceived != null && this.RaiseEvents)
                        {
                            var message = this.GetMessage();
                            if (message != null)
                            {
                                OnMessageReceived(new MessageReceivedEventArgs<T>(message));
                            }
                        }
                    }
                }
            }, cancellationToken);
        }
    

        private IEnumerable<bool> ReadQueue()
        {
            yield return readQueue;
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

        /// <summary>
        /// Read the message from the queue
        /// </summary>
        /// <returns>Instance of object T fetched from the queue</returns>
        public T GetMessage()
        {
            if (messageQueue != null)
            {
                if (this.readTimeout.HasValue)
                {
                    return this.messageFormatter.Read(messageQueue.Receive(TimeSpan.FromMilliseconds(this.readTimeout.Value))) as T;
                }
                else
                {
                    return this.messageFormatter.Read(messageQueue.Receive()) as T;
                }
            }
            return null;
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
                //cancellationTokenSource.Cancel();
                messageQueue.Dispose();
            }
        }

        #endregion

    }
}
