using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Globalization;
using ConsoleUtilities;

namespace diruse
{
    class Program
    {
        //private static decimal biggestFile = 0;
        enum outputFormat { Bytes, KiloBytes, MegaBytes, GigaBytes };


        static void Main(string[] args)
        {

            double spaceUsed;
            var directoryToCheck = "";
            bool recursionFlag = false;
            bool verboseFlag = false;
            outputFormat outFormat = outputFormat.Bytes;




            //bool findBiggestFileFlag = false;

            if(args.Length > 4 || args.Length < 1)
            {
                printUsage();
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
                    outFormat = outputFormat.KiloBytes;

                if (args[i].ToUpper().ToString() == "-B")
                    outFormat = outputFormat.Bytes;

                if (args[i].ToUpper().ToString() == "-M")
                    outFormat = outputFormat.MegaBytes;

                if (args[i].ToUpper().ToString() == "-G")
                    outFormat = outputFormat.GigaBytes;

            }


            if (args.Length > 1 && args[1] == "-R")recursionFlag = true;



            if (File.Exists(directoryToCheck))
            {

                //they gave us a file, just get how big it is.
                FileInfo fi = new FileInfo(directoryToCheck);
                Console.Write(fi.Length.ToString("N0", CultureInfo.InvariantCulture));
            }
            else
            {
                spaceUsed = checkDirectory(directoryToCheck, recursionFlag, verboseFlag, true, outFormat);
                if (verboseFlag)
                {
                    ConsoleUtils.WriteTwoColumns("Total:", GetFormattedNumber(spaceUsed,outFormat));
                }
                else
                {
                    ConsoleUtils.WriteTwoColumns("", GetFormattedNumber(spaceUsed, outFormat));
                }

            }

        }

        private static string GetFormattedNumber(double input, outputFormat outFormat)
        {
            string returnString;

            switch(outFormat)
            {
                case outputFormat.Bytes:
                    returnString = input.ToString("N0",CultureInfo.InvariantCulture);
                    break;
                case outputFormat.KiloBytes:
                    returnString = (input / 1024).ToString("N2",CultureInfo.InvariantCulture);
                    break;
                case outputFormat.MegaBytes:
                    returnString = ((input /1024)/1024).ToString("N2",CultureInfo.InvariantCulture);
                    break;
                case outputFormat.GigaBytes:
                    returnString = ((input / 1024) / 1024 / 1024).ToString("N2", CultureInfo.InvariantCulture);
                    break;
                default:
                    returnString = input.ToString("N0", CultureInfo.InvariantCulture);
                    break;
            }
            return returnString;

        }


        private static double checkDirectory(string directory, bool recurse, bool verbose, bool rootDirectory, outputFormat outFormat)
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
                            directorySize += checkDirectory(directory + "\\" + subDirectory.ToString(), true, verbose,false, outFormat);
                        }

                    }
                    catch (UnauthorizedAccessException)
                    {
                        //swallow it and keep going!
                        if (verbose && !(rootDirectory)) ConsoleUtils.WriteTwoColumns("Can't access", directory);
                    }
                }
                FileInfo[] files = di.GetFiles();
                foreach (FileInfo file in files)
                {
                    directorySize += file.Length;
                }
            }
            if (verbose) ConsoleUtils.WriteTwoColumns(directory, GetFormattedNumber(directorySize, outFormat));
            return directorySize;
        }



        private static void printUsage()
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
