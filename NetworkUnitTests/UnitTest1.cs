using System;
using NUnit.Framework;
using CS300Net;
using System.Runtime.Serialization;
using System.Text;

namespace NetworkUnitTests
{
    [TestFixture]
    public class NetworkUnitTests
    {
        private NetworkManager netMan = new NetworkManager();
        private class TestObserver : NetObserver
        {
            public byte[] dataRecieved = null;
            public bool recieved = false;

            public void ConnectionClosed(string ipAddr)
            {
                //throw new NotImplementedException();
            }

            public void ConnectionOpened(string ipAddr)
            {
                //throw new NotImplementedException();
            }

            public void DataRecieved(string ipAddr, byte[] data)
            {
                dataRecieved = new byte[data.Length];
                data.CopyTo(dataRecieved, 0);
                foreach (byte aByte in data)
                    Console.Write(aByte);
                Console.WriteLine();
                foreach (byte aByte in dataRecieved)
                    Console.Write(aByte);
                recieved = true;
            }

            public void Reset()
            {
                dataRecieved = null;
                recieved = false;
            }
        }
        private TestObserver obs = new TestObserver();

        [SetUp]
        public void Init()
        {
            //netMan = new NetworkManager();
            obs.Reset();
            netMan.Register(obs);
        }

        [TearDown]
        public void Cleanup()
        {
            netMan.StopListen();
            netMan.Disconnect();
        }

        [Test]
        [Property("ConnectTest", 1)]
        public void ConnectSelf()
        {
            netMan.Listen();

            if (!netMan.Connect(NetworkManager.LocalIP))
                Assert.Fail("Failed to connect to self");
        }

        [Test]
        [Property("ConnectTest", 1)]
        public void SendRecieveData()
        {
            netMan.Listen();
            byte[] data = Encoding.ASCII.GetBytes("Hello, World!");

            if (!netMan.Connect(NetworkManager.LocalIP))
                Assert.Fail("Failed to connect to self");

            if (!netMan.Send(NetworkManager.LocalIP, data))
                Assert.Fail("Failed to send data");

            while (!obs.recieved);

            Console.WriteLine("pre: {0}, post: {0}", data, obs.dataRecieved);
            for (int i = 0; i < data.Length && i < obs.dataRecieved.Length; ++i)
            {
                Assert.That(data[i] == obs.dataRecieved[i]);
            }
        }

        [Test]
        [Property("ConnectTest", 1)]
        public void SendRecieveEncodedData()
        {
            netMan.Listen();
            int[] intArr = { 8, 9, 10 };
            TestObject serializableObj = new TestObject('1', 2, 3, 4, 4f, "seven", intArr);

            if (!netMan.Connect(NetworkManager.LocalIP))
                Assert.Fail("Failed to connect to self");

            byte[] data = NetworkManager.Encode(serializableObj);

            if (!netMan.Send(NetworkManager.LocalIP, data))
                Assert.Fail("Failed to send data");

            while (!obs.recieved);

            TestObject post = NetworkManager.Decode<TestObject>(obs.dataRecieved);

            for (int i = 0; i < data.Length && i < obs.dataRecieved.Length; ++i)
            {
                Console.WriteLine("pre: {0}", data[i]);
                Console.WriteLine("post: {0}", obs.dataRecieved[i]);
                Assert.That(data[i] == obs.dataRecieved[i], "Data not the same here {0}: pre: {0} post: {0}", i);
            }

            Console.WriteLine("Pre: {0}", serializableObj.ToString());
            Console.WriteLine("Post: {0}", post.ToString());

            Assert.That(post.Equals(serializableObj));
        }

        [Test]
        public void ConnectInvalidAddress()
        {
            bool connected = netMan.Connect("bad address");
            Assert.That(connected == false);
        }

        [Test]
        public void ConnectNullAddress()
        {
            try
            {
                netMan.Connect(null);
            }
            catch (Exception e)
            {
                Assert.That(e.GetType() == typeof(ArgumentNullException));
                return;
            }

            Assert.Fail("No exception thrown for null address");
        }

        [Test]
        public void SendInvalidAddress()
        {
            byte[] data = { 1, 2, 3, 4 };
            try
            {
                netMan.Send("bad addr", data);
            }
            catch(Exception e)
            {
                Assert.That(e is InvalidOperationException);
                return;
            }

            Assert.Fail("No exception thrown for bad address");
        }

        [Test]
        public void SendNullAddress()
        {
            byte[] data = { 1, 2, 3, 4 };
            try
            {
                netMan.Send(null, data);
            }
            catch(Exception e)
            {
                Assert.That(e is ArgumentNullException, "Wrong exception type thrown, was {0}", e.GetType().ToString());
                Assert.That((e as ArgumentNullException).ParamName == "destIP", "NullException thrown for wrong argument, was {0}", (e as ArgumentNullException).ParamName);
                return;
            }
            Assert.Fail("No exception thrown for null address");
        }

        [Test]
        public void SendNullData()
        {
            try
            {
                netMan.Send("nonnulladdr", null);
            }
            catch (Exception e)
            {
                Assert.That(e is ArgumentNullException, "Wrong exception type thrown, was {0}", e.GetType());
                Assert.That((e as ArgumentNullException).ParamName == "data", "NullException thrown for wrong argument, was {0}", (e as ArgumentNullException).ParamName);
                return;
            }
            Assert.Fail("No exception thrown for null data");
        }

        [Test]
        public void EncodeData()
        {
            int[] arr = { 5, 6, 7, 8, 9 };
            TestObject pre = new TestObject('1', 2, 3, 4, 1.5f, "Hello, World!", arr);

            byte[] encoded = NetworkManager.Encode(pre);
            TestObject post = NetworkManager.Decode<TestObject>(encoded);

            Console.WriteLine(pre);
            Console.WriteLine(post);

            Assert.That(pre.Equals(post));
        }

        [Test]
        public void EncodeNullData()
        {
            TestObject pre = null;
            
            try
            {
                NetworkManager.Encode(pre);
            }
            catch(Exception e)
            {
                Assert.That(e is ArgumentNullException, "Wrong exception type thrown, was {0}", e.GetType());
                Assert.That((e as ArgumentNullException).ParamName == "obj");
                return;
            }

            Assert.Fail("No exception thrown on null obj argument");
        }

        [Test]
        public void EncodeUnserializable()
        {
            UnserializableObject pre = new UnserializableObject();

            try
            {
                NetworkManager.Encode(pre);
            }
            catch(Exception e)
            {
                Assert.That(e is SerializationException, "Wrong exception type thrown, was {0}", e.GetType());
                return;
            }

            Assert.Fail("No exception thrown on unserializable obj argument");
        }
    }

    [Serializable]
    public class TestObject
    {
        char charfield;
        short shortfield;
        int intfield;
        long longfield;
        float floatfield;
        string stringfield;
        int[] array;

        public TestObject(char aChar, short aShort, int aInt, long aLong, float aFloat, string aString, int[] anArray)
        {
            this.charfield = aChar;
            this.shortfield = aShort;
            this.intfield = aInt;
            this.longfield = aLong;
            this.floatfield = aFloat;
            this.stringfield = aString;
            array = new int[anArray.Length];
            anArray.CopyTo(array, 0);
        }

        public bool Equals(TestObject obj)
        {
            if (this.array.Length != obj.array.Length)
                return false;

            for(int i = 0; i < this.array.Length; ++i)
            {
                if (this.array[i] != obj.array[i])
                    return false;
            }

            return (this.charfield == obj.charfield &&
                    this.shortfield == obj.shortfield &&
                    this.intfield == obj.intfield &&
                    this.longfield == obj.longfield &&
                    this.floatfield == obj.floatfield &&
                    this.stringfield == obj.stringfield);
        }

        public override string ToString()
        {
            return string.Format("Char: {0}, ", charfield) + string.Format("Short: {0}, ", shortfield) + 
                string.Format("Int: {0}, ", intfield) +string.Format("Long: {0}, ", longfield) + 
                string.Format("Float: {0}, ", floatfield) + "String: " + stringfield + string.Format(" Array: {0}", array);
        }
    }

    public class UnserializableObject
    {
        int field1;
        int field2;

        public UnserializableObject()
        {
            field1 = 1;
            field2 = 2;
        }
    }
}
