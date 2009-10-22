using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace VirtualBicycle
{
    [StructLayout(LayoutKind.Sequential)]
    struct NativeRectangle
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct NativeMessage
    {
        public IntPtr hWnd;
        public uint msg;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public Point p;
    }

    [StructLayout(LayoutKind.Sequential)]
    struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public int showCmd;
        public Point ptMinPosition;
        public Point ptMaxPosition;
        public NativeRectangle rcNormalPosition;

        public static int Length
        {
            get { return Marshal.SizeOf(typeof(WINDOWPLACEMENT)); }
        }
    }
}
