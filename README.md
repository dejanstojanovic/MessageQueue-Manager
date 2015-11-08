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

## Benefits

- Lower size of the message stored in a message queue thanks to custom Json Message Formatter
- Faster writing of the message to message queue thanks to [NetJSON](https://github.com/rpgmaker/NetJSON)
- Faster reading of the message from the queue thanks to [NetJSON](https://github.com/rpgmaker/NetJSON)
- Direct cast of the message to object instance of specific type using generic type on the constructor
- Optimized overridable multi-threaded rwading of the message from the queue

![ScreenShot](http://dejanstojanovic.net/media/114939/performances.png)

Available as [NuGet package](https://www.nuget.org/packages/MessageQueueManager/)
```
PM> Install-Package MessageQueueManager
```
