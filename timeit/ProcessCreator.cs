using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace timeit {
 


public class ProcessCreator {


        [DllImport("user32.dll")]
        static extern uint WaitForInputIdle(IntPtr hProcess, uint dwMilliseconds);

        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool CreateProcess(
            string lpApplicationName, string lpCommandLine, ref SECURITY_ATTRIBUTES lpProcessAttributes,
            ref SECURITY_ATTRIBUTES lpThreadAttributes, bool bInheritHandles, uint dwCreationFlags,
            IntPtr lpEnvironment, string lpCurrentDirectory, [In] ref STARTUPINFOEX lpStartupInfo,
            out PROCESS_INFORMATION lpProcessInformation);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UpdateProcThreadAttribute(
            IntPtr lpAttributeList, uint dwFlags, IntPtr Attribute, IntPtr lpValue,
            IntPtr cbSize, IntPtr lpPreviousValue, IntPtr lpReturnSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool InitializeProcThreadAttributeList(
            IntPtr lpAttributeList, int dwAttributeCount, int dwFlags, ref IntPtr lpSize);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DeleteProcThreadAttributeList(IntPtr lpAttributeList);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(IntPtr hObject);

        public static  uint INFINITE = 0xFFFFFFFF;// Infinite timeout

        public static bool CreateProcess(int parentProcessId,string fullPath) {


/*
The only reason I wrote this is because we were having a problem with a machine that
took FOREVER to start an app, and we wanted to quantify how long it took so we could
tell our IT department to fix their shit.


This function is basically setting up and starting an app, and then waiting for it to finish
loading (i.e. WaitForInputIdle)
*/


            const uint EXTENDED_STARTUPINFO_PRESENT = 0x00080000;
            const int PROC_THREAD_ATTRIBUTE_PARENT_PROCESS = 0x00020000;

            var pInfo = new PROCESS_INFORMATION();
            var sInfoEx = new STARTUPINFOEX();
            sInfoEx.StartupInfo.cb = Marshal.SizeOf(sInfoEx);
            IntPtr lpValue = IntPtr.Zero;

            try {
                if (parentProcessId > 0) {
                    var lpSize = IntPtr.Zero;
                    var success = InitializeProcThreadAttributeList(IntPtr.Zero, 1, 0, ref lpSize);
                    if (success || lpSize == IntPtr.Zero) {
                        return false;
                    }

                    sInfoEx.lpAttributeList = Marshal.AllocHGlobal(lpSize);
                    success = InitializeProcThreadAttributeList(sInfoEx.lpAttributeList, 1, 0, ref lpSize);
                    if (!success) {
                        return false;
                    }

                    var parentHandle = Process.GetProcessById(parentProcessId).Handle;
                    // This value should persist until the attribute list is destroyed using the DeleteProcThreadAttributeList function
                    lpValue = Marshal.AllocHGlobal(IntPtr.Size);
                    Marshal.WriteIntPtr(lpValue, parentHandle);

                    success = UpdateProcThreadAttribute(
                        sInfoEx.lpAttributeList,
                        0,
                        (IntPtr)PROC_THREAD_ATTRIBUTE_PARENT_PROCESS,
                        lpValue,
                        (IntPtr)IntPtr.Size,
                        IntPtr.Zero,
                        IntPtr.Zero);
                    if (!success) {
                        return false;
                    }
                }

                var pSec = new SECURITY_ATTRIBUTES();
                var tSec = new SECURITY_ATTRIBUTES();
                pSec.nLength = Marshal.SizeOf(pSec);
                tSec.nLength = Marshal.SizeOf(tSec);
                var lpApplicationName = fullPath;

                if (!File.Exists(fullPath)) {
                    Console.WriteLine($"Can't find {fullPath}");
                    return false;
                }

                var sw = new Stopwatch();
                sw.Start();
                var returnVal = CreateProcess(lpApplicationName, null, ref pSec, ref tSec, false, EXTENDED_STARTUPINFO_PRESENT, IntPtr.Zero, null, ref sInfoEx, out pInfo);
                WaitForInputIdle(pInfo.hProcess, INFINITE);
                sw.Stop();

                Console.WriteLine($"CreateProcess returned {returnVal}");
                Console.WriteLine($"Operation took {sw.ElapsedMilliseconds}ms.");
                return true;

            } finally {
                // Free the attribute list
                if (sInfoEx.lpAttributeList != IntPtr.Zero) {
                    DeleteProcThreadAttributeList(sInfoEx.lpAttributeList);
                    Marshal.FreeHGlobal(sInfoEx.lpAttributeList);
                }
                Marshal.FreeHGlobal(lpValue);

                // Close process and thread handles
                if (pInfo.hProcess != IntPtr.Zero) {
                    CloseHandle(pInfo.hProcess);
                }
                if (pInfo.hThread != IntPtr.Zero) {
                    CloseHandle(pInfo.hThread);
                }
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct STARTUPINFOEX {
            public STARTUPINFO StartupInfo;
            public IntPtr lpAttributeList;
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct STARTUPINFO {
            public Int32 cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public Int32 dwX;
            public Int32 dwY;
            public Int32 dwXSize;
            public Int32 dwYSize;
            public Int32 dwXCountChars;
            public Int32 dwYCountChars;
            public Int32 dwFillAttribute;
            public Int32 dwFlags;
            public Int16 wShowWindow;
            public Int16 cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct PROCESS_INFORMATION {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }
    }
}
