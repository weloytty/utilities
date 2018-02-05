using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;

namespace ValidateJson
{
    class Program
    {
        static void Main(string[] args)
        {
            int argCount = args.Count();

            if (argCount != 2)
            {
                PrintUsage();
                return;
            }

            string inputFile = args[0];
            string schemaFile = args[1];

            Console.WriteLine("Input : {0}", inputFile);
            Console.WriteLine("Schema: {0}", schemaFile);
            if (!File.Exists(inputFile)) { throw new FileNotFoundException("Can't find {0}", inputFile); }
            if (!File.Exists(schemaFile)) { throw new FileNotFoundException("Can't find {0}", schemaFile); }

            string inputJson = File.ReadAllText(inputFile);
            string inputSchema = File.ReadAllText(schemaFile);

            JArray input = JArray.Parse(inputJson);
            

            JSchema schema = JSchema.Parse(inputSchema);

            IList<string> errorMessages;
            bool validJson = input.IsValid(schema, out errorMessages);

            Console.WriteLine("Json is {0}", (validJson ? "valid" : "not valid"));
            if (!validJson)
            {
                Console.WriteLine("Error messages:");
                foreach (string s in errorMessages)
                {
                    Console.WriteLine(s);
                }
            }
        }
        private static void PrintUsage()
        {
            Console.WriteLine("");
            Console.WriteLine("ValidateJson   : Validates a given Json file against a schema");
            Console.WriteLine("          Usage: ValidateJson PathToInputFile PathToSchema");
            Console.WriteLine("PathToInputFile: Where the input is.");
            Console.WriteLine("PathToSchema   : Where the schema is.");
            Console.WriteLine("");


        }

    }
}
