using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueuing.Routing
{
    public interface IMessageRouter<T>
    {
        string FallbackQueue { get; }
        IEnumerable<IMessageRoute<T>> Routes { get; }
        void Route(T message);


     
    }
}
