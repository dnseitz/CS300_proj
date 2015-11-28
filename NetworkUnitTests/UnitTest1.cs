using System;
using NUnit.Framework;
using CS300Net;
using System.Runtime.Serialization;

namespace NetworkUnitTests
{
    [TestFixture]
    public class NetworkUnitTests
    {
        private NetworkManager netMan = new NetworkManager();
        
        [SetUp]
        public void Init()
        {
            netMan = new NetworkManager();
        }

        [TearDown]
        public void Cleanup()
        {
            netMan.StopListen();
            netMan.Disconnect();
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
    internal class TestObject
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

    internal class UnserializableObject
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
