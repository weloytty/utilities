using System;
using System.Text;

namespace ConsoleUtilities
{
    public static class ConsoleUtils
    {


        #region Private Functions
        private static string Indent(int howMany)
        {
            return "".PadRight(howMany);
        }

        #endregion
        #region Public Functions
        public static void WriteBoldLine(string outputFormat, string outputString)
        { 
            Console.BackgroundColor = ConsoleColor.White;
            Console.WriteLine(outputFormat, outputString);
            Console.ResetColor();
        }


        public static void WriteBold(string outputFormat, string outputString)
        {
            Console.BackgroundColor = ConsoleColor.White;
            Console.Write(outputFormat, outputString);
            Console.ResetColor();
        }

        public static void WriteTwoColumns(string columnOne,string columnTwo,char spacesCharacter)
        {
            int consoleWidth = Console.WindowWidth;
            
            if (columnOne.Length >= (consoleWidth/2)-1) columnOne = columnOne.Substring(0, (int)(consoleWidth/ 2) -1);
            if (columnTwo.Length >= (consoleWidth / 2) - 1) columnTwo = columnTwo.Substring(0, (int)(consoleWidth/2) -1);
            
            String inBetween = new String(spacesCharacter, consoleWidth - (columnOne.Length + columnTwo.Length +1));
            Console.WriteLine(columnOne + inBetween  + columnTwo);

        }

        public static void WriteLeftColumn(string leftColumn,char spacesCharacter, bool isBold)
        {
            int consoleWidth = Console.WindowWidth;

            if(leftColumn.Length >= (consoleWidth/2)-1) leftColumn = leftColumn.Substring(0,(int)consoleWidth/2);
            String inBetween = new String(spacesCharacter,consoleWidth /2 - leftColumn.Length);
            if (isBold) Console.ForegroundColor = ConsoleColor.White;
            Console.Write(leftColumn + inBetween);
            if (isBold) Console.ResetColor();

        }

        public static void WriteRightColumn(string rightColumn, char spacesCharacter, bool isBold)
        { 
            int consoleWidth = Console.WindowWidth;

            if (rightColumn.Length >= (consoleWidth / 2) - 1)  rightColumn = rightColumn.Substring(0,(int)consoleWidth /2);
            
            String inBetween = new String(spacesCharacter,consoleWidth/2-rightColumn.Length);
            Console.Write(inBetween);
            if (isBold) Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(rightColumn);
            if (isBold) Console.ResetColor();
        }

        public static void WriteTwoColumns(string columnOne, string columnTwo)
        {
            WriteTwoColumns(columnOne, columnTwo, ' ');
        }
        #endregion




    }
}
