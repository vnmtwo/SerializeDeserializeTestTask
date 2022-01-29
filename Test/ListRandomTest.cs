using NUnit.Framework;
using SerializeDeserializeTestTask;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Test
{
    public class ListRandomTest
    {

        [Test]
        public void SerializeDeserialize_Normal()
        {
            var lr = GetTestListNode();
            var lr2 = new ListRandom();

            SerializeDeserialize(lr, lr2);

            Assert.AreEqual(lr.Count, lr2.Count);

            Assert.AreEqual(lr2[1], lr2.Head.Next);
            Assert.AreEqual(null, lr2.Head.Previous);
            Assert.AreEqual(null, lr2.Head.Random);
            Assert.AreEqual("1", lr2.Head.Data);

            Assert.AreEqual(null, lr2.Tail.Next);
            Assert.AreEqual(lr2[2], lr2.Tail.Previous);
            Assert.AreEqual(null, lr2.Tail.Random);
            Assert.AreEqual("4444", lr2.Tail.Data);

            lr2.Head.Data = null;
            lr2.Tail.Data = null;
            lr2.Head.Random = lr2.Tail;
            lr2.Tail.Random = lr2.Head;

            SerializeDeserialize(lr2, lr);

            Assert.AreEqual(lr.Count, lr2.Count);

            Assert.AreEqual(lr[1], lr.Head.Next);
            Assert.AreEqual(null, lr.Head.Previous);
            Assert.AreEqual(lr[3], lr.Head.Random);
            Assert.AreEqual(null, lr.Head.Data);

            Assert.AreEqual(null, lr.Tail.Next);
            Assert.AreEqual(lr[2], lr.Tail.Previous);
            Assert.AreEqual(lr[0], lr.Tail.Random);
            Assert.AreEqual(null, lr.Tail.Data);
        }

        [Test]
        public void SerializeDeserialize_NormalManyElements()
        {
            var lr = new ListRandom();
            var lr2 = new ListRandom();

            lr.Head.Data = "1";
            lr.Tail.Data = "10";
            for (int i = 9; i > 1; i--)
                lr.InsertElement(1, i.ToString());

            SerializeDeserialize(lr, lr2);

            Assert.AreEqual(lr.Count, lr2.Count);
            CollectionAssert.AreEqual(
                Enumerable.Range(1,10).Select(x=>x.ToString()).ToList(),
                lr2.GetElements());
        }

        [Test]
        public void Head_Exceptions()
        {
            var lr = new ListRandom();

            Assert.Throws<ArgumentNullException>(() => lr.Head = null);
            Assert.Throws<Exception>(() => lr.Head = new ListNode(null));
            Assert.Throws<Exception>(() => lr.Head = new ListNode("")
            {
                Next = new ListNode(""),
                Previous = new ListNode(""),
                Random = new ListNode("")
            });
        }

        [Test]
        public void Tail_Exceptions()
        {
            var lr = new ListRandom();

            Assert.Throws<ArgumentNullException>(() => lr.Tail = null);
            Assert.Throws<Exception>(() => lr.Tail = new ListNode(null));
            Assert.Throws<Exception>(() => lr.Tail = new ListNode("")
            {
                Next = new ListNode(""),
                Previous = new ListNode(""),
                Random = new ListNode("")
            });
        }

        [Test]
        public void InsertElement_Normal()
        {
            var lr = new ListRandom();
            var lr2 = new ListRandom();

            lr.InsertElement(1, "data");
            var el = lr[1].Random;
            int i = 0;
            if (el == null) i = 0;
            if (el == lr.Head) i = 1;
            if (el == lr.Tail) i = 2;            

            SerializeDeserialize(lr, lr2);

            Assert.AreEqual("data", lr2[1].Data);
            Assert.AreEqual(lr2.Head, lr2[1].Previous);
            Assert.AreEqual(lr2.Tail, lr2[1].Next);
            Assert.AreEqual(
                new ListNode[] {null, lr2.Head, lr2.Tail}[i],
                lr2[1].Random);
        }

        [Test]
        public void InsertElement_ArgumentOutOfRangeException()
        {
            var lr = new ListRandom();

            Assert.Throws<ArgumentOutOfRangeException>(() =>
                lr.InsertElement(0, "data"));
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                lr.InsertElement(2, "data"));
        }

        [Test]
        public void Serialize_Normal()
        {
            ListRandom lr = GetTestListNode();

            using (var s = new MemoryStream())
            {
                s.Seek(0, SeekOrigin.Begin);
                
                lr.Serialize(s);

                s.Seek(0, SeekOrigin.Begin);
                
                byte[] buffer = new byte[s.Length];
                s.Read(buffer);

                CollectionAssert.AreEqual(GetTestSerializedData(), buffer);
            }
        }

        [Test]
        public void Serialize_Exceprions()
        {
            var lr = new ListRandom();

            Assert.Throws<ArgumentNullException>(() => lr.Serialize(null));

            using (var s = new MemoryStream())
            {
                s.Close();
                Assert.Throws<NotSupportedException>(() => lr.Serialize(s));
            }
        }

        [Test]
        public void Deserialize_Normal()
        {
            using var s = new MemoryStream();
            s.Write(GetTestSerializedData());
            s.Seek(0, SeekOrigin.Begin);
            var lr = new ListRandom();
            lr.Deserialize(s);

            Assert.AreEqual(4, lr.Count);

            Assert.AreEqual(lr[1], lr.Head.Next);
            Assert.AreEqual(null, lr.Head.Previous);
            Assert.AreEqual(null, lr.Head.Random);
            Assert.AreEqual("1", lr.Head.Data);

            Assert.AreEqual(null, lr.Tail.Next);
            Assert.AreEqual(lr[2], lr.Tail.Previous);
            Assert.AreEqual(null, lr.Tail.Random);
            Assert.AreEqual("4444", lr.Tail.Data);

        }

        [Test]
        public void Deserialize_Exceptions()
        {
            var lr = new ListRandom();
            
            using (var s = new MemoryStream())
            {
                lr.Serialize(s);
                s.Write(new byte[18], 0, 18);
                s.Seek(0, SeekOrigin.Begin);

                Assert.Throws<ArgumentNullException>(() => lr.Deserialize(null));
                Assert.Throws<Exception>(() => lr.Deserialize(s));
                
                s.Close();
               
                Assert.Throws<NotSupportedException>(() => lr.Deserialize(s));
            }

            using (var s = new MemoryStream())
            {
                s.Seek(0, SeekOrigin.Begin);
                s.Write(new byte[3], 0, 3);

                Assert.Throws<Exception>(() => lr.Deserialize(s));
            }

            
        }

        [Test]
        public void GetElement_Normal()
        {
            var lr = new ListRandom();

            Assert.AreEqual(lr.Head, lr[0]);
            Assert.AreEqual(lr.Tail, lr[lr.Count - 1]);
        }

        [Test]
        public void GetElements_Normal()
        {
            //all code copied from SerializeDeserialize_NormalManyElements()
            var lr = new ListRandom();
            var lr2 = new ListRandom();

            lr.Head.Data = "1";
            lr.Tail.Data = "10";
            for (int i = 9; i > 1; i--)
                lr.InsertElement(1, i.ToString());

            SerializeDeserialize(lr, lr2);

            Assert.AreEqual(lr.Count, lr2.Count);
            CollectionAssert.AreEqual(
                Enumerable.Range(1, 10).Select(x => x.ToString()).ToList(),
                lr2.GetElements());
        }

        private static void SerializeDeserialize(ListRandom from, ListRandom to)
        {
            using (var s = new MemoryStream())
            {
                s.Seek(0, SeekOrigin.Begin);
                from.Serialize(s);

                s.Seek(0, SeekOrigin.Begin);
                to.Deserialize(s);
            }
        }

        private static ListRandom GetTestListNode()
        {
            var lr = new ListRandom();
            lr.Head.Data = "1";
            lr.Tail.Data = "4444";
            lr.InsertElement(1, "22");
            lr.InsertElement(2, "333");

            foreach (var e in lr)
                e.Random = null;

            return lr;
        }

        private static byte[] GetTestSerializedData()
        {
            var bl = new List<byte>();

            bl.AddRange(BitConverter.GetBytes(4));

            bl.AddRange(BitConverter.GetBytes(-1)); bl.AddRange(BitConverter.GetBytes(2));
            bl.AddRange(BitConverter.GetBytes(-1)); bl.AddRange(BitConverter.GetBytes(1 * 2));
            bl.AddRange(BitConverter.GetBytes('1'));

            bl.AddRange(BitConverter.GetBytes(3)); bl.AddRange(BitConverter.GetBytes(-1));
            bl.AddRange(BitConverter.GetBytes(-1)); bl.AddRange(BitConverter.GetBytes(4 * 2));
            bl.AddRange(BitConverter.GetBytes('4'));
            bl.AddRange(BitConverter.GetBytes('4'));
            bl.AddRange(BitConverter.GetBytes('4'));
            bl.AddRange(BitConverter.GetBytes('4'));

            bl.AddRange(BitConverter.GetBytes(0)); bl.AddRange(BitConverter.GetBytes(3));
            bl.AddRange(BitConverter.GetBytes(-1)); bl.AddRange(BitConverter.GetBytes(2 * 2));
            bl.AddRange(BitConverter.GetBytes('2'));
            bl.AddRange(BitConverter.GetBytes('2'));

            bl.AddRange(BitConverter.GetBytes(2)); bl.AddRange(BitConverter.GetBytes(1));
            bl.AddRange(BitConverter.GetBytes(-1)); bl.AddRange(BitConverter.GetBytes(3 * 2));
            bl.AddRange(BitConverter.GetBytes('3'));
            bl.AddRange(BitConverter.GetBytes('3'));
            bl.AddRange(BitConverter.GetBytes('3'));
            
            return bl.ToArray();
        }

    }
}