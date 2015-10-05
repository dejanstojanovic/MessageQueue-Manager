using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueuing
{

    /// <summary>
    /// Represents custom event argument which contains object instance of type T read from the message queue
    /// </summary>
    /// <typeparam name="T">Type of the messages read from the message queue</typeparam>
    public class MessageReceivedEventArgs<T> : EventArgs
    {
        #region Fields
        private T message;
        #endregion

        #region Properies
        /// <summary>
        /// Message of type T read from the message queue
        /// </summary>
        public T Message
        {
            get
            {
                return this.message;
            }
        }
        #endregion

        #region Constructors
        public MessageReceivedEventArgs(T message)
        {
            this.message = message;
        }
        #endregion
    }
}
