# MessageQueue-Manager
Event driven class for reading messages from MSMQ

Create instance of the MessageQueueManager class for the type T and attach handle to MessageReceived event

```cs
MessageQueueManager queueManager = new MessageQueueManager<SampleModel>(@".\private$\TestQueue");
queueManager.MessageReceived += QueueManager_MessageReceived;
```


Actual handle method which will be triggered once the message of the type T is received
```cs
 private static void QueueManager_MessageReceived(object sender, MessageReceivedEventArgs<SampleModel> e)
        {
            Console.WriteLine("Message received {0} {1}", e.Message.ID, e.Message.TimeCreated.ToString("yyyy-MM-dd HH:mm:ss.fff"));
        }
```
