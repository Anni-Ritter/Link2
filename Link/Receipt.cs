using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Link
{
    [Serializable]
    public class Receipt
    {
        public Receipt() { }

        public Receipt(int id, BitArray array)
        {
            Id = id;
            Status = array;
        }
        public Receipt(BitArray status)
        {
            Status = status;
        }
        public int Id { get; set; }
        public BitArray Status { get; set; }
     }
}
