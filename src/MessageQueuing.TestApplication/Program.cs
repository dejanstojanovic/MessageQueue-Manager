using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageQueuing.TestApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            var queueManager = new MessageQueueManager<SampleModel>(@".\private$\TestQueue");

            //for(int i = 0; i < 13; i++)
            //while(true)
            //{
            //    queueManager.AddMessage(new SampleModel() { ID = Guid.NewGuid().ToString(), TimeCreated=DateTime.Now });
            //}

            queueManager.MessageReceived += QueueManager_MessageReceived;

            Console.ReadLine();
            queueManager.Dispose();
            Console.ReadLine();
        }

        private static void QueueManager_MessageReceived(object sender, MessageReceivedEventArgs<SampleModel> e)
        {
            Console.WriteLine("Message received {0} {1}", e.Message.ID, e.Message.TimeCreated.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }
    }
}
