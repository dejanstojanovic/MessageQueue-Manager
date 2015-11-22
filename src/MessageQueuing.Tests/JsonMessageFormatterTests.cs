using System;
using System.Messaging;
using MessageQueuing;
using MessageQueuing.Tests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace MessageQueuing.Tests
{
    [TestClass]
   public class JsonMessageFormatterTests
    {
        [TestMethod]
        public void TestFormatterSerialization()
        {
            JsonMessageFormatter<TestModel> writeFormatter = new JsonMessageFormatter<TestModel>(Encoding.UTF8);
            TestModel writeObject = new TestModel() { ID = Guid.NewGuid().ToString(), TimeCreated = DateTime.Now };
            Message message = new Message(writeObject, writeFormatter);
            writeFormatter.Write(message,writeObject);

            JsonMessageFormatter<TestModel> readFormatter = new JsonMessageFormatter<TestModel>(Encoding.UTF8);
            TestModel readObject = readFormatter.Read(message) as TestModel;

            string dateFormat = "yyyy-MM-dd HH:mm:ss.fff";

            Assert.IsTrue(readObject != null &&
                writeObject.ID == readObject.ID &&
                writeObject.TimeCreated.ToString(dateFormat) == readObject.TimeCreated.ToString(dateFormat));

        }
    }
}
