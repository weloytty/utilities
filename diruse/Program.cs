using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
using System.Linq;
using ConsoleUtilities;

namespace diruse
{
    class Program
    {
        //private static decimal biggestFile = 0;
        enum OutputFormat { Bytes, KiloBytes, MegaBytes, GigaBytes };


        static void Main(string[] args)
        {
            var directoryToCheck = "";
            bool recursionFlag = false;
            bool verboseFlag = false;
            OutputFormat outFormat = OutputFormat.Bytes;




            //bool findBiggestFileFlag = false;

            if(args.Length > 4 || args.Length < 1)
            {
                PrintUsage();
                return;
            }


            //Console.WriteLine("\n");
            directoryToCheck = args[0].ToString();

            for (int i = 1; i < args.Length; i++)
            {
                if (args[i].ToUpper().ToString() == "-R")
                    recursionFlag = true;

                if (args[i].ToUpper().ToString() == "-V")
                    verboseFlag = true;

                if (args[i].ToUpper().ToString() == "-K")
                    outFormat = OutputFormat.KiloBytes;

                if (args[i].ToUpper().ToString() == "-B")
                    outFormat = OutputFormat.Bytes;

                if (args[i].ToUpper().ToString() == "-M")
                    outFormat = OutputFormat.MegaBytes;

                if (args[i].ToUpper().ToString() == "-G")
                    outFormat = OutputFormat.GigaBytes;

            }


            if (args.Length > 1 && args[1] == "-R")recursionFlag = true;

            if (File.Exists(directoryToCheck))
            {

                //they gave us a file, just get how big it is.
                FileInfo fi = new FileInfo(directoryToCheck);
                Console.Write(fi.Length.ToString("N0", CultureInfo.InvariantCulture));
            }
            else {
                var spaceUsed = CheckDirectory(directoryToCheck, recursionFlag, verboseFlag, true, outFormat);
                ConsoleUtils.WriteTwoColumns(verboseFlag ? "Total:" : "", GetFormattedNumber(spaceUsed, outFormat));
            }

        }

        private static string GetFormattedNumber(double input, OutputFormat outFormat) {
            var returnString = outFormat switch {
                OutputFormat.Bytes => input.ToString("N0", CultureInfo.InvariantCulture),
                OutputFormat.KiloBytes => (input / 1024).ToString("N2", CultureInfo.InvariantCulture),
                OutputFormat.MegaBytes => ((input / 1024) / 1024).ToString("N2", CultureInfo.InvariantCulture),
                OutputFormat.GigaBytes => ((input / 1024) / 1024 / 1024).ToString("N2", CultureInfo.InvariantCulture),
                _ => input.ToString("N0", CultureInfo.InvariantCulture)
            };
            return returnString;
        }


        private static double CheckDirectory(string directory, bool recurse, bool verbose, bool rootDirectory, OutputFormat outFormat)
        {

            double directorySize = 0;

            if (Directory.Exists(directory))
            {
                DirectoryInfo di = new DirectoryInfo(directory);

                if (recurse)
                {
                    DirectoryInfo[] subDirectories = di.GetDirectories();
                    try
                    {
                        foreach (DirectoryInfo subDirectory in subDirectories)
                        {
                            directorySize += CheckDirectory(directory + "\\" + subDirectory.ToString(), true, verbose,false, outFormat);
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        //swallow it and keep going!
                        if (verbose && !(rootDirectory)) ConsoleUtils.WriteTwoColumns("Can't access", directory);
                    }
                }
                FileInfo[] files = di.GetFiles();
                directorySize = files.Aggregate(directorySize, (current, file) => current + file.Length);
            }
            if (verbose) ConsoleUtils.WriteTwoColumns(directory, GetFormattedNumber(directorySize, outFormat));
            return directorySize;
        }



        private static void PrintUsage()
        {
            Console.WriteLine("\nUsage:  diruse [path] [-R] [-K|B|M]");
            Console.WriteLine("  [path]  = path to show");
            Console.WriteLine("  [-R]    = recurse into subdirectories");
            Console.WriteLine("  [-V]    = verbose output\n");
            Console.WriteLine("  [-K|B|M|G]= output answer in bytes, kilobytes, megabytes, or gigabytes");
            Console.WriteLine("            (default is KB)");


        }
    }
}
