namespace SerializeDeserializeTestTask
{
    //serialize struct: 
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
}
