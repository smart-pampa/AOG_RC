using System;
using System.Collections.Generic;
using RateController.Domain;
using RateController.PGNs;

namespace RateController
{
    public class PGN235 : PGN
    {
        // section widths
        // 0    header Hi       128 0x80
        // 1    header Lo       129 0x81
        // 2    source          126 0x7E
        // 3    AGIO PGN        235 0xEB
        // 4    length          33
        // 5-36 sections 0-15, width in cm
        // 37   # of sections
        // 38   CRC

        private byte cSectionCount;
        private readonly int[] cWidth_cm = new int[16];

        public PGN235()
        {
        }

        public event EventHandler SectionsChanged;

        public void ParseByteData(byte[] Data)
        {
            bool ValidData = true;
            int Width;
            if (Data.Length > 5)
            {
                if (Data.Length == Data[4] + 6)
                {
                    if (GoodCRC(Data, 2))
                    {
                        cSectionCount = Data[37];
                        if (cSectionCount > 0 && cSectionCount < 17)
                        {
                            for (int i = 0; i < cSectionCount; i++)
                            {
                                Width = Data[i * 2 + 5] + (Data[i * 2 + 6] << 8);
                                if (Width < 0 || Width > 5000)
                                {
                                    ValidData = false;
                                    break;
                                }
                                cWidth_cm[i] = Width;
                            }
                            if (ValidData)
                            {
                                if (Changed()) SectionsChanged?.Invoke(this, EventArgs.Empty);
                            }
                        }
                    }
                }
            }
        }

        public byte SectionCount()
        {
            return cSectionCount;
        }

        public int Width_cm(int SectionID)
        {
            return cWidth_cm[SectionID];
        }

        private bool Changed(List<Section> lstSections)
        {
            bool Result = false;
            if (lstSections.Count != cSectionCount)
            {
                Result = true;
            }
            else
            {
                for (int i = 0; i < cSectionCount; i++)
                {
                    if (cWidth_cm[i] != lstSections[i].Width_cm)
                    {
                        Result = true;
                        break;
                    }
                }
            }
            return Result;
        }
    }
}