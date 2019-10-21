using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace WEL {
    public static class JsonUtils {
        public class ValidationResult {
            public bool IsValid;
            public IList<string> ValidationResults;
        }

        public static ValidationResult IsValidJson(string inputFile, string schemaFile) {

            string inputJson = File.ReadAllText(inputFile);
            string inputSchema = File.ReadAllText(schemaFile);

            JArray input = JArray.Parse(inputJson);
            JSchema schema = JSchema.Parse(inputSchema);

            bool validJson = input.IsValid(schema, out IList<string> errorMessages);

            return new ValidationResult() { IsValid = validJson, ValidationResults = errorMessages };


        }
    }
}
