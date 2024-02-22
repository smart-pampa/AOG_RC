using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RateController.PGNs
{
    public abstract class PGN
    {
        protected byte[] cData;
        public PGN() { }

        public byte[] Data { get { return cData; } }

        public bool GoodCRC(byte[] Data, byte Start = 0)
        {
            bool Result = false;
            int Length = Data.Length;
            byte cr = CRC(Data, Length - 1, Start);
            Result = (cr == Data[Length - 1]);
            return Result;
        }

        public byte CRC(byte[] Data, int Length, byte Start = 0)
        {
            byte Result = 0;
            if (Length <= Data.Length)
            {
                int CK = 0;
                for (int i = Start; i < Length; i++)
                {
                    CK += Data[i];
                }
                Result = (byte)CK;
            }
            return Result;
        }

        public bool BitRead(byte b, int pos)
        {
            return ((b >> pos) & 1) != 0;
        }


    }
}
