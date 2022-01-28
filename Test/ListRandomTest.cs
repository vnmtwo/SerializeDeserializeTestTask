using NUnit.Framework;
using SerializeDeserializeTestTask;
using System.Collections.Generic;
using System.IO;

namespace Test
{
    public class ListRandomTest
    {

        [Test]
        public void SerializeDeserialize_Normal2Elements()
        {
            var lr = new ListRandom();
            var lr2 = new ListRandom();

            lr.Head.Data = "123";
            lr.Tail.Data = "234";

            lr.Head.Random = lr.Tail;
            lr.Tail.Random = lr.Head;

            using (var s = new MemoryStream())
            {
                s.Seek(0, SeekOrigin.Begin);
                lr.Serialize(s);
                s.Seek(0, SeekOrigin.Begin);
                lr2.Deserialize(s);
            }

            Assert.AreEqual(lr.Count, lr2.Count);

            Assert.AreEqual(lr2.Tail, lr2.Head.Next);
            Assert.AreEqual(null, lr2.Head.Previous);
            Assert.AreEqual(lr2.Tail, lr2.Head.Random);
            Assert.AreEqual("123", lr2.Head.Data);

            Assert.AreEqual(null, lr2.Tail.Next);
            Assert.AreEqual(lr2.Head, lr2.Tail.Previous);
            Assert.AreEqual(lr2.Head, lr2.Tail.Random);
            Assert.AreEqual("234", lr2.Tail.Data);

            lr2.Head.Data = null;
            lr2.Tail.Data = null;
            lr2.Head.Random = null;
            lr2.Tail.Random = null;

            using (var s = new MemoryStream())
            {
                s.Seek(0, SeekOrigin.Begin);
                lr2.Serialize(s);
                s.Seek(0, SeekOrigin.Begin);
                lr.Deserialize(s);
            }

            Assert.AreEqual(lr2.Count, lr.Count);

            Assert.AreEqual(lr.Tail, lr.Head.Next);
            Assert.AreEqual(null, lr.Head.Previous);
            Assert.AreEqual(null, lr.Head.Random);
            Assert.AreEqual(null, lr.Head.Data);

            Assert.AreEqual(null, lr.Tail.Next);
            Assert.AreEqual(lr.Head, lr.Tail.Previous);
            Assert.AreEqual(null, lr.Tail.Random);
            Assert.AreEqual(null, lr.Tail.Data);
        }

        [Test]
        public void SerializeDeserialize_Normal3Elements()
        {
            var lr = new ListRandom();
            var lr2 = new ListRandom();

            lr.Head.Data = "1";
            lr.Tail.Data = "3";
            lr.InsertElement(1, "2");

            using (var s = new MemoryStream())
            {
                s.Seek(0, SeekOrigin.Begin);
                lr.Serialize(s);
                s.Seek(0, SeekOrigin.Begin);
                lr2.Deserialize(s);
            }

            Assert.AreEqual(lr.Count, lr2.Count);

            Assert.AreNotEqual(lr2.Tail, lr2.Head.Next);
            Assert.AreEqual(null, lr2.Head.Previous);
            Assert.AreEqual("1", lr2.Head.Data);

        }

        [Test]
        public void SerializeDeserialize_NormalManyElements()
        {
            var lr = new ListRandom();
            var lr2 = new ListRandom();

            lr.Head.Data = "1";
            lr.Tail.Data = "10";
            lr.InsertElement(1, "9");
            lr.InsertElement(1, "8");
            lr.InsertElement(1, "7");
            lr.InsertElement(1, "6");
            lr.InsertElement(1, "5");
            lr.InsertElement(1, "4");
            lr.InsertElement(1, "3");
            lr.InsertElement(1, "2");

            using (var s = new MemoryStream())
            {
                s.Seek(0, SeekOrigin.Begin);
                lr.Serialize(s);
                s.Seek(0, SeekOrigin.Begin);
                lr2.Deserialize(s);
            }

            Assert.AreEqual(lr.Count, lr2.Count);
            CollectionAssert.AreEqual(new List<string>() { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" },
                    lr2.GetElements());
        }

    }
}