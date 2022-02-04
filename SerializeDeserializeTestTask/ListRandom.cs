﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace SerializeDeserializeTestTask
{
    public class ListRandom : IEnumerable
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
                if (value is null) throw new ArgumentNullException();
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
                if (value is null) throw new ArgumentNullException();
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
        public ListNode this[int index]
        {
            get
            {
                int c=0;
                foreach(var e in this)
                {
                    if (c == index) return e;
                    c++;
                }
                throw new Exception("get element exception");
            }
            private set { }

        }

        private Random rnd;

        public ListRandom()
        {
            Init();
        }

        private void Init()
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

            ListNode Prev = null, Next = null, node = new ListNode(value);
            int c = 0;

            foreach (var n in this)
            {
                if (index == c + 1) Prev = n;
                if (index == c) { Next = n; break; }
                c++;
            }

            Next.Previous = node;
            Prev.Next = node;
            node.Previous = Prev;
            node.Next = Next;

            int i = rnd.Next(Count - 1) - 1; //last -1 for negative number and null generation
            node.Random = (i < 0) ? null : Store[i];

            Store.Add(node);
        }

        public void Serialize(Stream s)
        {
            if (s is null) throw new ArgumentNullException("s");
            if (!s.CanWrite) throw new NotSupportedException("s is unwritable");

            var bw = new BinaryWriter(s);
          
            bw.Write(Count);

            foreach (var cn in Store)
            {
                int prev = (cn.Previous is null) ? -1 : Store.IndexOf(cn.Previous);
                int next = (cn.Next is null) ? -1 : Store.IndexOf(cn.Next);
                int random = (cn.Random is null) ? -1 : Store.IndexOf(cn.Random);
                int datalength = (cn.Data is null) ? -1 : cn.Data.Length;

                bw.Write(prev);
                bw.Write(next);
                bw.Write(random);
                bw.Write(datalength * sizeof(char));
                if (cn.Data != null) bw.Write(cn.Data);
            }
        }

        public void Deserialize(Stream s)
        {
            if (s is null) throw new ArgumentNullException("s");
            if (!s.CanRead) throw new NotSupportedException("s is unreadable");

            if (s.Length < minStreamLength)
                throw new Exception("Stream is incorrect");

            var indexes = new List<TempNode>();

            var br = new BinaryReader(s);
          
            int count = br.ReadInt32();

            try
            {
                for (int i = 0; i < count; i++)
                {
                    int prev = br.ReadInt32();
                    int next = br.ReadInt32();
                    int random = br.ReadInt32();
                    int datalength = br.ReadInt32();
                    string data = null;

                    if (datalength >= 0) data = br.ReadString();
                    indexes.Add(new TempNode()
                    {
                        Node = new ListNode(data),
                        Next = next,
                        Previous = prev,
                        Random = random
                    });
                }
            }
            catch
            {
                throw new Exception("Stream is corrupt");
            }

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
            foreach (var e in this)
            {
                o.Add(e.Data);
            }
            return o;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)GetEnumerator();
        }

        public Enumerators.ListRandomEnum GetEnumerator()
        {
            return new Enumerators.ListRandomEnum(Head);
        }
    }
}
