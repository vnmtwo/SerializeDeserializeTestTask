using NUnit.Framework;
using SerializeDeserializeTestTask;
using System.IO;

namespace Test
{
    public class ListRandomTest
    {

        [Test]
        public void SerializeDeserialize_Normal()
        {
            var lr = new ListRandom();
            lr.Head = new ListNode();
            lr.Tail = new ListNode();

            lr.Head.Previous = null;
            lr.Head.Random = null;
            lr.Head.Next = lr.Tail;

            lr.Tail.Previous = lr.Head;
            lr.Tail.Next = null;
            lr.Tail.Random = lr.Head;

            lr.Head.Data = "123";
            lr.Tail.Data = "234";

            lr.Count = 2;

            var lr2 = new ListRandom();
            using (var s = new MemoryStream())
            {
                s.Seek(0, SeekOrigin.Begin);
                lr.Serialize(s);
                s.Seek(0, SeekOrigin.Begin);
                lr2.Deserialize(s);
            }

            Assert.AreEqual(lr.Count, lr2.Count);
            Assert.AreEqual(lr2.Tail, lr2.Head.Next);
            Assert.AreEqual(null, lr2.Head.Random);
            Assert.AreEqual(null, lr2.Head.Previous);
            Assert.AreEqual("123", lr2.Head.Data);

            Assert.AreEqual(null, lr2.Tail.Next);
            Assert.AreEqual(lr2.Head, lr2.Tail.Random);
            Assert.AreEqual(lr2.Head, lr2.Tail.Previous);
            Assert.AreEqual("234", lr2.Tail.Data);

            lr2.Head.Data = null;
            lr2.Tail.Data = null;

            using (var s = new MemoryStream())
            {
                s.Seek(0, SeekOrigin.Begin);
                lr2.Serialize(s);
                s.Seek(0, SeekOrigin.Begin);
                lr.Deserialize(s);
            }

            Assert.AreEqual(lr2.Count, lr.Count);
            Assert.AreEqual(lr.Tail, lr.Head.Next);
            Assert.AreEqual(null, lr.Head.Random);
            Assert.AreEqual(null, lr.Head.Previous);
            Assert.AreEqual(null, lr.Head.Data);

            Assert.AreEqual(null, lr.Tail.Next);
            Assert.AreEqual(lr.Head, lr.Tail.Random);
            Assert.AreEqual(lr.Head, lr.Tail.Previous);
            Assert.AreEqual(null, lr.Tail.Data);
        }
    }
}