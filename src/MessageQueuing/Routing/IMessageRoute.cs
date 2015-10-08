using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueuing.Routing
{
    public interface IMessageRoute<T>
    {
        int Order { get; }
        string DestinationQueue { get; }

        bool ProcessMessage(T message);
    }
}
