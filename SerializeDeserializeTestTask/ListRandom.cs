using System;
using System.Collections.Generic;
using System.IO;

namespace SerializeDeserializeTestTask
{
    public class ListNode
    {
        public ListNode Previous;
        public ListNode Next;
        public ListNode Random; // произвольный элемент внутри списка
        public string Data;
    }

    //struct: 
    //prev: int
    //next: int
    //random: int
    //datalength: int
    //data:string?

    internal struct TempNode
    {
        public ListNode Node;
        public int Next, Previous, Random;
    }

    public class ListRandom
    {
        public ListNode Head;
        public ListNode Tail;
        public int Count; // change minStreamLength if you will change type, because i cant use marshalling

        private const int minStreamLength = (sizeof(int) * 4) * 2 + sizeof(int); //struct*2 + Count;

        public void Serialize(Stream s)
        {
            if (Head is null) throw new NullReferenceException();

            s.Write(BitConverter.GetBytes(Count));
            if (Count > 0)
            {
                var indexes = new Dictionary<ListNode, int>();
                int c = 0;
                ListNode currentNode = Head;
                do
                {
                    indexes.Add(currentNode, c++);
                    currentNode = currentNode.Next;
                } while (currentNode != null);

                currentNode = Head;
                do
                {
                    int prev, next, random, datalength;

                    if (currentNode.Previous == null) prev = -1;
                    else prev = indexes[currentNode.Previous];

                    if (currentNode.Next == null) next = -1;
                    else next = indexes[currentNode.Next];

                    if (currentNode.Random == null) random = -1;
                    else random = indexes[currentNode.Random];

                    if (currentNode.Data == null) datalength = -1;
                    else datalength = currentNode.Data.Length;

                    s.Write(BitConverter.GetBytes(prev));
                    s.Write(BitConverter.GetBytes(next));
                    s.Write(BitConverter.GetBytes(random));
                    s.Write(BitConverter.GetBytes(datalength * sizeof(char)));

                    if (datalength > 0)
                    {
                        foreach (var ch in currentNode.Data)
                        {
                            s.Write(BitConverter.GetBytes(ch));
                        }
                    }

                    currentNode = currentNode.Next;

                } while (currentNode != null);
            }
        }



        public void Deserialize(Stream s)
        {
            if (s == null)
                throw new ArgumentNullException("s");

            if (s.Length < minStreamLength)
                throw new Exception("Stream is incorrect");

            byte[] buffer = new byte[sizeof(int)];

            s.Read(buffer, 0, buffer.Length);
            Count = BitConverter.ToInt32(buffer, 0);

            int prev, next, random, datalength;
            string data;

            var indexes = new List<TempNode>();

            while (s.Read(buffer, 0, sizeof(int)) == buffer.Length)
            {
                prev = BitConverter.ToInt32(buffer, 0);

                s.Read(buffer, 0, buffer.Length);
                next = BitConverter.ToInt32(buffer, 0);

                s.Read(buffer, 0, buffer.Length);
                random = BitConverter.ToInt32(buffer, 0);

                s.Read(buffer, 0, buffer.Length);
                datalength = BitConverter.ToInt32(buffer, 0);

                var chars = new List<char>();

                if (datalength / sizeof(char) == -1) data = null;
                else
                {
                    buffer = new byte[sizeof(char)];
                    for (int i = 0; i < datalength / sizeof(char); i++)
                    {
                        s.Read(buffer, 0, buffer.Length);
                        chars.Add(BitConverter.ToChar(buffer, 0));
                    }
                    data = new string(chars.ToArray());
                }

                indexes.Add(new TempNode() { Node = new ListNode() { Data = data }, Next = next, Previous = prev, Random = random });

                buffer = new byte[sizeof(int)];
            }

            if (indexes.Count != Count) throw new Exception("Stream is corrupt");
            for (int i = 0; i < indexes.Count; i++)
            {
                FillNode(indexes, i);
            }
        }

        private void FillNode(List<TempNode> indexes, int i)
        {
            var tempNode = indexes[i];
            var node = tempNode.Node;

            if (i == 0) Head = node;
            if (i == indexes.Count - 1) Tail = node;

            if (tempNode.Previous == -1) node.Previous = null;
            else node.Previous = indexes[tempNode.Previous].Node;

            if (tempNode.Random == -1) node.Random = null;
            else node.Random = indexes[tempNode.Random].Node;

            if (tempNode.Next == -1) node.Next = null;
            else node.Next = indexes[tempNode.Next].Node;
        }
    }
}
