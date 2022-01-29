using System;
using System.Collections;

namespace SerializeDeserializeTestTask.Enumerators
{
    public class ListRandomEnum : IEnumerator
    {
        private ListNode head;
        private ListNode current;
        private bool f;

        public ListRandomEnum(ListNode head)
        {
            f = true;
            this.head = head;
            this.current = null;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public ListNode Current
        {
            get
            {
                if (f) throw new InvalidOperationException();
                return current;
            }
        }

        public bool MoveNext()
        {
            if (f)
            {
                current = head;
                f = false;
                return true;
            }
            if (Current.Next is null) return false;

            current = current.Next;
            return true;
        }

        public void Reset()
        {
            f = true;
            this.current = head;
        }
    }
}
