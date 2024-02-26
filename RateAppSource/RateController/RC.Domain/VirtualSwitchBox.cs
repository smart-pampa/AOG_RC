﻿using System;
using RateController.PGNs;

namespace RateController.Domain
{
    public class VirtualSwitchBox
    {
        // to Rate Controller from virtual switch box
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

        private bool cAuto;
        private bool cEnabled;
        private bool cLargeScreenOn;
        private bool[] cSwitch;
        private bool cSwitchScreenOn;
        private byte[] PressedData;
        private System.Windows.Forms.Timer Timer1 = new System.Windows.Forms.Timer();

        public VirtualSwitchBox()
        {
            cSwitch = new bool[Configuration.MaxSwitches];
            PressedData = new byte[6];
            PressedData[0] = 106;
            PressedData[1] = 127;
            Timer1.Tick += new EventHandler(TimerEventProcessor);
            Timer1.Interval = 250;
        }

        /* Funcion UDPconn que elimine:
public bool SwitchBoxConnected
{ get { return ((DateTime.Now - SBtime).TotalSeconds < 4); } }
*/
        private bool phySwitchboxConnected()
        {
            bool Result = false;
            if (1 == 1) //mf.UDPmodules.SwitchBoxConnected)
            {
                Result = true;
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    if (1==1) //(mf.SER[i].SwitchBoxConnected)
                    {
                        Result = true;
                        break;
                    }
                }
            }
            return Result;
        }

        public bool Enabled
        {
            get { return cEnabled; }
            set
            {
                if (value && !phySwitchboxConnected())
                {
                    Timer1.Start();
                    cEnabled = true;
                }
                else
                {
                    if ((!cSwitchScreenOn && !cLargeScreenOn) || phySwitchboxConnected())  // disable if physical switchbox connected
                    {
                        Timer1.Stop();
                        cEnabled = false;
                    }
                }
            }
        }

        public bool LargeScreenOn
        {
            get { return cLargeScreenOn; }
            set
            {
                cLargeScreenOn = value;
                Enabled = value;
            }
        }

        public bool SwitchScreenOn
        {
            get { return cSwitchScreenOn; }
            set
            {
                cSwitchScreenOn = value;
                Enabled = value;
            }
        }

        public void PressSwitch(SwIDs ID, bool FromLargeScreen = false)
        {
            PGN32618 msgPGN = new PGN32618();
            // build PGN32618
            switch (ID)
            {
                case SwIDs.Auto:
                    if (cAuto)
                    {
                        // turn off
                        PressedData[2] = msgPGN.BitClear(PressedData[2], 0);
                        cAuto = false;
                    }
                    else
                    {
                        // turn on
                        PressedData[2] = msgPGN.BitSet(PressedData[2], 0);
                        cAuto = true;
                    }
                    break;

                case SwIDs.MasterOn:
                    PressedData[2] = msgPGN.BitSet(PressedData[2], 1);
                    PressedData[2] = msgPGN.BitClear(PressedData[2], 2);
                    if (FromLargeScreen)
                    {
                        PressedData[3] = 255;
                        PressedData[4] = 255;
                        for (int i = 0; i < Configuration.MaxSwitches; i++)
                        {
                            cSwitch[i] = true;
                        }
                    }
                    break;

                case SwIDs.MasterOff:
                    PressedData[2] = msgPGN.BitClear(PressedData[2], 1);
                    PressedData[2] = msgPGN.BitSet(PressedData[2], 2);
                    break;

                case SwIDs.RateUp:
                    PressedData[2] = msgPGN.BitSet(PressedData[2], 3);
                    PressedData[2] = msgPGN.BitClear(PressedData[2], 4);
                    break;

                case SwIDs.RateDown:
                    PressedData[2] = msgPGN.BitClear(PressedData[2], 3);
                    PressedData[2] = msgPGN.BitSet(PressedData[2], 4);
                    break;

                default:
                    // switches
                    int Num = (int)ID - 5;

                    if (Num < 8)
                    {
                        if (cSwitch[Num])
                        {
                            // turn off, lo
                            PressedData[3] = msgPGN.BitClear(PressedData[3], Num);
                            cSwitch[Num] = false;
                        }
                        else
                        {
                            // turn on, lo
                            PressedData[3] = msgPGN.BitSet(PressedData[3], Num);
                            cSwitch[Num] = true;
                        }
                    }
                    else
                    {
                        if (cSwitch[Num])
                        {
                            // turn off, hi
                            PressedData[4] = msgPGN.BitClear(PressedData[4], Num - 8);
                            cSwitch[Num] = false;
                        }
                        else
                        {
                            // turn on, hi
                            PressedData[4] = msgPGN.BitSet(PressedData[4], Num - 8);
                            cSwitch[Num] = true;
                        }
                    }
                    break;
            }
            PressedData[5] = msgPGN.CRC(PressedData, 5);
        }

        public void ReleaseSwitch()
        {
            PGN32618 msgPGN = new PGN32618();

            PressedData[2] = (byte)(PressedData[2] & 1);
            PressedData[5] = msgPGN.CRC(PressedData, 5);
        }

        private void TimerEventProcessor(Object myObject, EventArgs myEventArgs)
        {
            SwitchBox.ParseByteData(PressedData);
        }
    }
}