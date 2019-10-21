using System;
using System.IO;
using System.Linq;

namespace WEL.JsonValidator
{
    class Program
    {
        static void Main(string[] args)
        {
            int argCount = args.Count();

            PrintBanner();

            if (argCount != 2)
            {
                PrintUsage();
                return;
            }


            string inputFile = args[0];
            string schemaFile = args[1];

            if (!File.Exists(inputFile)) { throw new FileNotFoundException($"Can't find '{inputFile}'"); }
            if (!File.Exists(schemaFile)) { throw new FileNotFoundException($"Can't find '{schemaFile}'"); }

            var retVal = WEL.JsonUtils.IsValidJson(inputFile,schemaFile);
            if (retVal.IsValid) {
                Console.WriteLine("File validated successfully");
            } else {
                Console.WriteLine($"{retVal.ValidationResults.Count} errors validating {inputFile}");
                foreach (var rr in retVal.ValidationResults) {
                    Console.WriteLine(rr);
                }
            }
        }

        private static void PrintBanner() {
            Console.WriteLine("");
            Console.WriteLine("ValidateJson   : Validates a given Json file against a schema");
            Console.WriteLine($"(c) 2016-{DateTime.Now.Year} Bill Loytty");
        }

        private static void PrintUsage()
        {
            
            Console.WriteLine("          Usage: ValidateJson PathToInputFile PathToSchema");
            Console.WriteLine("PathToInputFile: Where the input is.");
            Console.WriteLine("PathToSchema   : Where the schema is.");
            Console.WriteLine("");


        }

    }
}
