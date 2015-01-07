using System;
using System.Collections.Generic;
using System.Text;

namespace eflayMH_WPF
{
    class War3Version
    {
        public static bool IsVs124e(ProcessC war3)
        {
            byte[] bt = new byte[4];
            war3.ReadMemory(new IntPtr(war3.DllBaseAddress + 0x0fb76b), bt, 4);
            byte[] bt2 = new byte[] { 0xe8, 0x30, 0x96, 0xff };

            for (int i = 0; i < 4; i++)
            {
                if (bt[i] == bt2[i])
                { }
                else
                { return false; }
            }
            return true;
        }

        public static bool IsVs120e(ProcessC war3)
        {
            byte[] bt = new byte[4];
            war3.ReadMemory(new IntPtr(war3.DllBaseAddress + 0x0e35d8), bt, 4);
            byte[] bt2 = new byte[] { 0xe8, 0xf3, 0x1F, 0xF8 };

            for (int i = 0; i < 4; i++)
            {
                if (bt[i] == bt2[i])
                { }
                else
                { return false; }
            }
            return true;
        }

        public static string GetWar3Version(ProcessC war3)
        {
            if (IsVs124e(war3))
            {
                return "1.24.4.6387";
            }
            if (IsVs120e(war3))
            {
                return "1.20.4.6074";
            }
            else
            {
                return "";
            }
        }
    }
}
