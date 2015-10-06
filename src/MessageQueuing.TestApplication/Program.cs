using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueuing.TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {

            using (var queueManager = new MessageQueueManager<SampleModel>(@".\private$\TestQueue", new JsonMessageFormatter()))
            {
                queueManager.AddMessage(new SampleModel() { ID = Guid.NewGuid().ToString(), TimeCreated = DateTime.Now });

            }

            using (var queueManager = new MessageQueueManager<SampleModel>(@".\private$\TestQueue", new XmlMessageFormatter()))
            {
                queueManager.AddMessage(new SampleModel() { ID = Guid.NewGuid().ToString(), TimeCreated = DateTime.Now });

            }

        }

        private static void QueueManager_MessageReceived(object sender, MessageReceivedEventArgs<SampleModel> e)
        {
            Console.WriteLine("Message received {0} {1}", e.Message.ID, e.Message.TimeCreated.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }
    }
}
