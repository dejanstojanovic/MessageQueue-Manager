using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            /*--------------------- SIZE TEST START ---------------------*/

            /* Using JSON formatter */
            
            //using (var queueManager = new MessageQueueManager<SampleModel>(@".\private$\TestQueue"))
            //{
            //    queueManager.AddMessage(new SampleModel() { ID = Guid.NewGuid().ToString(), TimeCreated = DateTime.Now });
            //}

            ///* Using XML formatter */
            //using (var queueManager = new MessageQueueManager<SampleModel>(@".\private$\TestQueue", new XmlMessageFormatter()))
            //{
            //    queueManager.AddMessage(new SampleModel() { ID = Guid.NewGuid().ToString(), TimeCreated = DateTime.Now });
            //}
            /*--------------------- SIZE TEST END ---------------------*/


            ///*--------------------- SPEED TEST START ---------------------*/
            //var JsonTimer = Stopwatch.StartNew();
            //new JsonMessageFormatter<SampleModel>().Write(new Message(), new SampleModel());
            //JsonTimer.Stop();

            //var XmlTimer = Stopwatch.StartNew();
            //new XmlMessageFormatter().Write(new Message(), new SampleModel());
            //XmlTimer.Stop();

            //Console.WriteLine("JSON: {0}", JsonTimer.ElapsedMilliseconds);
            //Console.WriteLine("XML: {0}", XmlTimer.ElapsedMilliseconds);
            ///*--------------------- SPEED TEST END ---------------------*/

            

            //Console.ReadLine();

            using (var queueManager = new MessageQueueManager<SampleModel>(@".\private$\TestQueue"))
            {
                //queueManager.MessageQueue.Purge();
                queueManager.RaiseEvents = true;
                queueManager.MessageReceived += QueueManager_MessageReceived;

                //while (true)
                //{
                //    queueManager.AddMessage(new SampleModel() { ID = Guid.NewGuid().ToString(), TimeCreated = DateTime.Now });
                //}
                Console.ReadLine();
            }
            Console.ReadLine();
        }

        private static void QueueManager_MessageReceived(object sender, MessageReceivedEventArgs<SampleModel> e)
        {
            Console.WriteLine("Message received {0} {1}", e.Message.ID, e.Message.TimeCreated.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }
    }
}
