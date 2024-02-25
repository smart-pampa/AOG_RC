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

        public bool GoodCRC(string[] Data, byte Start = 0)
        {
            bool Result = false;
            byte tmp;
            int Length = Data.Length;
            byte[] BD = new byte[Length];
            for (int i = 0; i < Length; i++)
            {
                if (byte.TryParse(Data[i], out tmp)) BD[i] = tmp;
            }
            byte cr = CRC(BD, Length - 1, Start);   // exclude existing crc
            Result = (cr == BD[Length - 1]);
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

        public byte CRC(string[] Data, int Length, byte Start = 0)
        {
            byte Result = 0;
            if (Length <= Data.Length)
            {
                byte tmp;
                byte[] BD = new byte[Length];
                for (int i = 0; i < Length; i++)
                {
                    if (byte.TryParse(Data[i], out tmp)) BD[i] = tmp;
                }
                int CK = 0;
                for (int i = Start; i < Length; i++)
                {
                    CK += BD[i];
                }
                Result = (byte)CK;
            }
            return Result;
        }

        public bool BitRead(byte b, int pos)
        {
            return ((b >> pos) & 1) != 0;
        }

        public byte BitClear(byte b, int pos)
        {
            byte msk = (byte)(1 << pos);
            msk = (byte)~msk;
            return (byte)(b & msk);
        }

        public byte BitSet(byte b, int pos)
        {
            return (byte)(b | (1 << pos));
        }

        public byte ParseModID(byte ID)
        {
            // top 4 bits
            return (byte)(ID >> 4);
        }

        public byte ParseSenID(byte ID)
        {
            // bottom 4 bits
            return (byte)(ID & 0b00001111);
        }
    }
}
