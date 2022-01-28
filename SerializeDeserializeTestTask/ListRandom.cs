﻿using System;
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

        public ListNode(string value)
        {
            Data = value;
        }
    }

    //struct: 
    //prev: int
    //next: int
    //random: int
    //datalength: int
    //data:string?

    internal class TempNode
    {
        public ListNode Node;
        public int Next, Previous, Random;
    }

    public class ListRandom
    {
        private const int minStreamLength = (sizeof(int) * 4) * 2 + sizeof(int); //struct*2 + Count;

        private List<ListNode> Store;

        private ListNode _head;
        public ListNode Head
        {
            get
            {
                return _head;
            }
            set
            {
                if (Store.Contains(value.Next))
                {
                    int i = Store.IndexOf(value.Next);
                    if (Store[i].Previous == value)
                    {
                        int indx = Store.IndexOf(_head);
                        Store[indx] = value;
                        _head = Store[indx];
                        return;
                    }
                }
                throw new Exception("Incorrect head");
            }
        }

        private ListNode _tail;
        public ListNode Tail
        {
            get
            {
                return _tail;
            }
            set
            {
                if (Store.Contains(value.Previous))
                {
                    int i = Store.IndexOf(value.Previous);
                    if (Store[i].Next == value)
                    {
                        int indx = Store.IndexOf(_tail);
                        Store[indx] = value;
                        _head = Store[indx];
                        return;
                    }
                }
                throw new Exception("Incorrect tail");

            }
        }

        public int Count // change minStreamLength if you will change type, because i cant use marshalling
        {
            get
            {
                return Store.Count;
            }
            private set
            {

            }
        }

        private Random rnd;

        public ListRandom()
        {
            Init();
            var l = new List<string>();
        }

        public void Init()
        {
            Store = new List<ListNode>();
            rnd = new Random();

            Store.Add(new ListNode(null));
            Store.Add(new ListNode(null));

            _head = Store[0];
            _tail = Store[1];

            _head.Next = _tail;
            _head.Random = rnd.Next(2) == 0 ? null : _tail;
            _head.Previous = null;

            _tail.Next = null;
            _tail.Random = rnd.Next(2) == 0 ? null : _head;
            _tail.Previous = _head;
        }

        public void InsertElement(int index, string value)
        {
            if (index <= 0 || index >= Store.Count)
                throw new ArgumentOutOfRangeException();
            ListNode node = new ListNode(value);

            ListNode currentNode = Head;
            ListNode nextNode = null;
            int c = 0;
            do
            {
                nextNode = currentNode.Next;
                if (index == c + 1)
                {
                    currentNode.Next = node;
                    node.Previous = currentNode;
                }
                if (index == c)
                {
                    currentNode.Previous = node;
                    node.Next = currentNode;
                    break;
                }
                c++;
                currentNode = nextNode;
            } while (nextNode != null);

            int i = rnd.Next(Count - 1) - 1;
            if (i < 0) node.Random = null;
            else node.Random = Store[i];

            Store.Add(node);
        }

        public void Serialize(Stream s)
        {
            if (Head is null) throw new NullReferenceException();

            s.Write(BitConverter.GetBytes(Count));
            if (Count > 0)
            {
                foreach (var cn in Store)
                {
                    int prev, next, random, datalength;

                    if (cn.Previous == null) prev = -1;
                    else prev = Store.IndexOf(cn.Previous);

                    if (cn.Next == null) next = -1;
                    else next = Store.IndexOf(cn.Next);

                    if (cn.Random == null) random = -1;
                    else random = Store.IndexOf(cn.Random);

                    if (cn.Data == null) datalength = -1;
                    else datalength = cn.Data.Length;

                    s.Write(BitConverter.GetBytes(prev));
                    s.Write(BitConverter.GetBytes(next));
                    s.Write(BitConverter.GetBytes(random));
                    s.Write(BitConverter.GetBytes(datalength * sizeof(char)));

                    if (datalength > 0)
                    {
                        foreach (var ch in cn.Data)
                        {
                            s.Write(BitConverter.GetBytes(ch));
                        }
                    }
                }
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
            var count = BitConverter.ToInt32(buffer, 0);

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

                indexes.Add(new TempNode() { Node = new ListNode(data), Next = next, Previous = prev, Random = random });

                buffer = new byte[sizeof(int)];
            }

            if (indexes.Count != count) throw new Exception("Stream is corrupt");

            Init();

            for (int i = 0; i < indexes.Count; i++)
            {
                FillNode(indexes, i);
            }
        }

        private void FillNode(List<TempNode> indexes, int i)
        {
            var tempNode = indexes[i];
            var node = tempNode.Node;

            if (tempNode.Previous == -1) { node.Previous = null; Store[0] = node; _head = Store[0]; }
            else node.Previous = indexes[tempNode.Previous].Node;

            if (tempNode.Random == -1) node.Random = null;
            else node.Random = indexes[tempNode.Random].Node;

            if (tempNode.Next == -1) { node.Next = null; Store[1] = node; _tail = Store[1]; }
            else node.Next = indexes[tempNode.Next].Node;

            if (node.Previous != null && node.Next != null)
                Store.Add(node);
        }

        public List<string> GetElements()
        {
            List<string> o = new List<string>();

            ListNode cn = Head;
            do
            {
                o.Add(cn.Data);
                cn = cn.Next;
            } while (cn != null);

            return o;
        }
    }
}
