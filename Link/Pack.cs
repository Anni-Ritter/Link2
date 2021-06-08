using System;
using System.Collections;

namespace Link
{
    [Serializable]
    public class Pack
    {
        public Pack() { }

        public Pack(int id, BitArray bitArray, int checkSum, int useful, BitArray status, int? repeatId)
        {
            Id = id;
            Data = bitArray;
            CheckSum = checkSum;
            UsefulData = useful;
            Status = status;
            RepeatId = repeatId;
        }
        public Pack(BitArray status)
        {
            Status = status;
        }
        public int Id { get; set; }
        public BitArray Data { get; set; }
        public int CheckSum { get; set; }
        public int UsefulData { get; set; }
        public BitArray Status { get; set; }
        public int? RepeatId { get; set; }
    }
}
