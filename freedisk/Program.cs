
using System;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;

namespace freedisk
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string wmiQuery = "SELECT * FROM Win32_LogicalDisk";
            string computerName = ".";
            bool noOutput = true;
            for (int index = 0; index < args.Length; ++index)
            {
                switch (args[index])
                {
                    case "-d":
                        if (args.Length > index + 1)
                        {
                            ++index;
                            string cleanName = Program.RemoveInvalidChars(args[index]);
                            wmiQuery = wmiQuery + $" WHERE Name = '{cleanName}'";
                        }
                        break;
                    case "-b":
                        noOutput = false;
                        break;
                    case "-m":
                        if (args.Length > index + 1)
                        {
                            ++index;
                            computerName = Program.RemoveInvalidChars(args[index]);
                        }
                        break;
                    default:
                        Program.ShowUsage();
                        return;
                }
            }
            try {
                
                PrintBanner();
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher(new ManagementScope("\\\\" + computerName + "\\root\\cimv2"), new SelectQuery(wmiQuery));
                Console.WriteLine("");
                if (noOutput)
                {
                    Console.WriteLine("Name Size (GB)   Free (GB)");
                    Console.WriteLine("---- ---------   --------");
                }
                foreach (ManagementObject managementObject in managementObjectSearcher.Get())
                {
                    ulong diskSize = Convert.ToUInt64(managementObject.GetPropertyValue("Size"));
                    ulong diskFree = Convert.ToUInt64(managementObject.GetPropertyValue("FreeSpace"));
                    if (diskSize > 0UL)
                        Console.WriteLine("{0}   {1}    {2}", managementObject.GetPropertyValue("Name"), 
                            Program.BytesToFormattedGB(diskSize, 8), 
                            Program.BytesToFormattedGB(diskFree, 8));
                }
                Console.WriteLine("");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
            
        }

        private static string BytesToFormattedGB(ulong bytes, int padding)
        {
            return Program.BytesToFormattedGB(bytes).PadLeft(padding, '0');
        }

        private static string BytesToFormattedGB(ulong bytes)
        {
            ulong bytesInGigabyte = 1073741824UL;
            return Convert.ToString(((float)bytes / (float)bytesInGigabyte).ToString("N", (IFormatProvider)Thread.CurrentThread.CurrentCulture));
        }

        private static string RemoveInvalidChars(string inputString)
        {
            return new Regex("\\<;-'").Replace(inputString, " ");
        }

        private static void PrintBanner() {
            Console.WriteLine("");
            Console.WriteLine($"freedisk.exe (c) 2014-{DateTime.Now.Year} Bill Loytty");
        }

        private static void ShowUsage() {
            
            PrintBanner();
            Console.WriteLine("USAGE: freedisk.exe [-d driveletter] [-b] [-m computername] [-?]");
            Console.WriteLine("       -d = Drive to check (default is all)");
            Console.WriteLine("       -b = Do not display headings");
            Console.WriteLine("       -m = Computer to check (default is .)");
            Console.WriteLine("       -? = Shows this help");
            Console.WriteLine("");
        }
    }
}
