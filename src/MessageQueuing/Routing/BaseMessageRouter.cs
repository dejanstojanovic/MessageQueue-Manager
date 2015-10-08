using MessageQueuing.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueuing.Routing
{
    public class BaseMessageRouter<T> : IMessageRouter<T>
    {
        private string fallbackQueue;
        IEnumerable<IMessageRoute<T>> routes;

        private MessageQueue fallbackQueueObject;
        public string FallbackQueue
        {
            get
            {
                return fallbackQueue;
            }
        }

        public IEnumerable<IMessageRoute<T>> Routes
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public BaseMessageRouter(IEnumerable<IMessageRoute<T>> routes, string fallbackQueue)
        {
            this.routes = routes;
            this.fallbackQueue = fallbackQueue;
            fallbackQueueObject = new MessageQueue(this.fallbackQueue);

        }

        public virtual void Route(T message)
        {
            bool routed = false;

            foreach(IMessageRoute<T> route in Routes)
            {
              bool accepted  =  route.ProcessMessage(message);
                if(!routed && accepted)
                {
                    routed = true;
                }
            }


        }
    }
}
