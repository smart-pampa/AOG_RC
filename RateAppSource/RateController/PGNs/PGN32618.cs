﻿using System;
using RateController.PGNs;

namespace RateController
{
    public enum SwIDs
    { Auto, MasterOn, MasterOff, RateUp, RateDown, sw0, sw1, sw2, sw3, sw4, sw5, sw6, sw7, sw8, sw9, sw10, sw11, sw12, sw13, sw14, sw15 };

    public class PGN32618 : PGN
    {
        // to Rate Controller from arduino switch box
        // 0   106
        // 1   127
        // 2    - bit 0 Auto
        //      - bit 1 MasterOn
        //      - bit 2 MasterOff
        //      - bit 3 RateUp
        //      - bit 4 RateDown
        // 3    sw0 to sw7
        // 4    sw8 to sw15
        // 5    crc

        private const byte cByteCount = 6;
        private const byte HeaderHi = 127;
        private const byte HeaderLo = 106;
        private bool cAutoOn = true;
        private bool cMasterOn = false;
        private bool cRateDown = false;
        private bool cRateUp = false;
        private DateTime ReceiveTime;
        private bool[] SW = new bool[21];

        public PGN32618()
        {
            SW[(int)SwIDs.Auto] = true; // default to auto in case of no switchbox
        }

        public event EventHandler<SwitchPGNargs> SwitchPGNreceived;

        public bool AutoOn
        {
            get { return cAutoOn; }
            set { cAutoOn = value; }
        }

        public bool MasterOn
        {
            get { return cMasterOn; }
            set { cMasterOn = value; }
        }

        public bool RateDown
        {
            get { return cRateDown; }
            set { cRateDown = value; }
        }

        public bool RateUp
        {
            get { return cRateUp; }
            set { cRateUp = value; }
        }

        public bool[] Switches
        { get { return SW; } }

        public bool Connected()
        {
            return (DateTime.Now - ReceiveTime).TotalSeconds < 4;
        }

        public bool ParseByteData(byte[] Data)
        {
            bool Result = false;

            if (Data[0] == HeaderLo && Data[1] == HeaderHi && Data.Length >= cByteCount && GoodCRC(Data))
            {
                // auto
                SW[0] = BitRead(Data[2], 0);
                cAutoOn = SW[0];

                // master
                SW[1] = BitRead(Data[2], 1);     // master on
                SW[2] = BitRead(Data[2], 2);     // master off

                if (SW[1]) cMasterOn = true;
                else if (SW[2]) cMasterOn = false;

                // rate
                SW[3] = BitRead(Data[2], 3);     // rate up
                SW[4] = BitRead(Data[2], 4);     // rate down

                if (SW[3])
                {
                    cRateUp = true;
                    cRateDown = false;
                }
                else if (SW[4])
                {
                    cRateUp = false;
                    cRateDown = true;
                }
                else if (!SW[3] && !SW[4])
                {
                    cRateUp = false;
                    cRateDown = false;
                }

                // section switches
                for (int i = 0; i < 2; i++)
                {
                    for (int j = 0; j < 8; j++)
                    {
                        SW[5 + j + i * 8] = BitRead(Data[i + 3], j);
                    }
                }

                SwitchPGNargs args = new SwitchPGNargs();
                args.Switches = SW;
                SwitchPGNreceived?.Invoke(this, args);

                ReceiveTime = DateTime.Now;
                Result = true;
            }
            return Result;
        }

        public bool ParseStringData(string[] Data)
        {
            bool Result = false;
            byte[] BD;
            if (Data.Length < 100)
            {
                BD = new byte[Data.Length];
                for (int i = 0; i < Data.Length; i++)
                {
                    byte.TryParse(Data[i], out BD[i]);
                }
                Result = ParseByteData(BD);
            }
            return Result;
        }

        public bool SectionSwitchOn(int ID)
        {
            bool Result = false;
            if ((ID >= 0) && (ID <= 15))
            {
                Result = SW[ID + (int)SwIDs.sw0];
            }
            return Result;
        }

        public bool SwitchIsOn(SwIDs ID)
        {
            return SW[(int)ID];
        }

        public class SwitchPGNargs : EventArgs
        {
            public bool[] Switches { get; set; }
        }
    }
}