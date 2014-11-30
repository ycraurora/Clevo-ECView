using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ECView.Tools
{
    public class CallingVariation
    {
        [DllImport("ecview.dll", EntryPoint = "#3")]
        public static extern void SetFanDuty(int p1, int p2);

        [DllImport("ecview.dll", EntryPoint = "#4")]
        public static extern int SetFANDutyAuto(int p1);

        [DllImport("ecview.dll", EntryPoint = "#5")]
        public static extern ECData GetTempFanDuty(int p1);

        [DllImport("ecview.dll", EntryPoint = "#6")]
        public static extern int GetFANCounter();

        [DllImport("ecview.dll", EntryPoint = "#8")]
        public static extern string GetECVersion();

        public struct ECData
        {
            public int data;
            public int data1;
            public int data2;
        }
    }
}
