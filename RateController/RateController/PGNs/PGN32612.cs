﻿using System;

namespace RateController
{
    public class PGN32612
    {
        // to Rate Controller from AOG
        //0	header 0        127
        //1	header 1        100
        //2	workedAreaTotal Hi      hectares X 100
        //3	workedAreaTotal Lo
        //4	ToolWorkingWidth Hi     only sections that are on, actual X 100
        //5	ToolWorkingWidth Lo
        //6	Speed Hi                actual X 100
        //7	Speed Lo
        //8	mdSectionControlByteHi
        //9	mdSectionControlByteLo

        public byte SectionControlByteHi;
        public byte SectionControlByteLo;
        private const byte cByteCount = 10;
        private const byte HeaderHi = 127;
        private const byte HeaderLo = 100;
        private byte SpeedHi;
        private byte SpeedLo;
        private int Temp;
        private byte WidthHi;
        private byte WidthLo;
        private byte WorkedHi;
        private byte WorkedLo;

        float KalResult = 0.0F;
        float KalPc = 0.0F;
        float KalG = 0.0F;
        float KalP = 1.0F;
        float KalVariance = 0.01F;   // larger is more filtering
        float KalProcess = 0.005F;  // smaller is more filtering

        private DateTime ReceiveTime;
        private byte LastSectionHi;
        private byte LastSectionLo;

        public event EventHandler<SectionsChangedEventArgs> SectionsChanged;

        public class SectionsChangedEventArgs: EventArgs
        {
            public byte SectionHi { get; set; }
            public byte SectionLo { get; set; }
        }

        public byte ByteCount()
        {
            return cByteCount;
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;
            if (Data[0] == HeaderHi & Data[1] == HeaderLo & Data.Length >= cByteCount)
            {
                WorkedHi = Data[2];
                WorkedLo = Data[3];
                WidthHi = Data[4];
                WidthLo = Data[5];
                SpeedHi = Data[6];
                SpeedLo = Data[7];
                SectionControlByteHi = Data[8];
                SectionControlByteLo = Data[9];

                if ((SectionControlByteHi != LastSectionHi) || (SectionControlByteLo != LastSectionLo))
                {
                    SectionsChangedEventArgs args = new SectionsChangedEventArgs();
                    args.SectionHi = SectionControlByteHi;
                    args.SectionLo = SectionControlByteLo;
                    SectionsChanged?.Invoke(this, args);

                    LastSectionHi = SectionControlByteHi;
                    LastSectionLo = SectionControlByteLo;
                }

                ReceiveTime = DateTime.Now;
                Result = true;
            }
            return Result;
        }

        public double Speed()
        {
            if (Connected())
            {
                Temp = SpeedHi << 8 | SpeedLo;
                //return Temp / 100.0;

                // Kalmen filter
                KalPc = KalP + KalProcess;
                KalG = KalPc / (KalPc + KalVariance);
                KalP = (1 - KalG) * KalPc;
                KalResult = (KalG * ((float)Temp - KalResult) + KalResult);
                return (double)(KalResult / 100.0);
            }
            else
            {
                return 0;
            }
        }

        public double WorkedArea()
        {
            Temp = WorkedHi << 8 | WorkedLo;
            return Temp / 100.0;
        }

        public double WorkingWidth()
        {
            Temp = WidthHi << 8 | WidthLo;
            return Temp / 100.0;
        }

        public bool Connected()
        {
            return ((DateTime.Now - ReceiveTime).TotalSeconds < 4);
        }

    }
}