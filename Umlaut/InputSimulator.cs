using System;
using System.Runtime.InteropServices;

namespace Umlaut
{
    public static class InputSimulator
    {

        public static void SendInput(char c)
        {
            UInt16 scanCode = c;

            INPUT[] inputs = new INPUT[2];
            inputs[0] = new INPUT()
            {
                Type = (UInt32)1,
                Data =
                {
                    Keyboard = new KEYBOARDINPUT
                    {
                        KeyCode = 0,
                        Scan = scanCode,
                        Flags = (UInt32)0x0004,
                        Time = 0,
                        ExtraInfo = IntPtr.Zero
                    }
                }
            };

            inputs[1] = new INPUT()
            {
                Type = (UInt32)1,
                Data =
                {
                    Keyboard = new KEYBOARDINPUT
                    {
                        KeyCode = 0,
                        Scan = scanCode,
                        Flags = (UInt32)(0x0002 | 0x0004),
                        Time = 0,
                        ExtraInfo = IntPtr.Zero
                    }
                }
            };

            if ((scanCode & 0xFF00) == 0xE000)
            {
                inputs[0].Data.Keyboard.Flags |= (UInt32)0x0001;
                inputs[1].Data.Keyboard.Flags |= (UInt32)0x0001;
            }

            NativeMethods.SendInput((UInt32)inputs.Length, inputs, Marshal.SizeOf(typeof(INPUT)));
        }

        private static class NativeMethods
        {
            [DllImport("user32.dll", SetLastError = true)]
            public static extern UInt32 SendInput(UInt32 numberOfInputs, INPUT[] inputs, Int32 sizeOfInputStructure);
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSEINPUT
        {
            public Int32 X;
            public Int32 Y;
            public UInt32 MouseData;
            public UInt32 Flags;
            public UInt32 Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct KEYBOARDINPUT
        {
            public UInt16 KeyCode;
            public UInt16 Scan;
            public UInt32 Flags;
            public UInt32 Time;
            public IntPtr ExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HARDWAREINPUT
        {
            public UInt32 Msg;
            public UInt16 ParamL;
            public UInt16 ParamH;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct INPUT
        {
            public UInt32 Type;
            public INPUTUNION Data;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct INPUTUNION
        {
            [FieldOffset(0)]
            public MOUSEINPUT Mouse;
            [FieldOffset(0)]
            public KEYBOARDINPUT Keyboard;
            [FieldOffset(0)]
            public HARDWAREINPUT Hardware;
        }
    }
}
