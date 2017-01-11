using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

// TODO: Actually implement the Regex parser to get the cost and paid amounts, calculate the change, and figure out the coins to give back.

/// <summary>
/// Currency based on the Penny with value 1.
/// A dollar is the same as 100 Pennies, so it has a value of 100.
/// </summary>
enum Currency : byte
{
    /// <summary>
    /// The base unit for Currency.
    /// </summary>
    Penny = 1,
    Nickel = 5,
    Dime = 10,
    Quarter = 25,
    Dollar = 100
}

namespace ChangeBack
{
    class Program
    {
        private static string CurrencySymbol = "$";
        private static string CurrencyPattern = @""; // TODO: Write the pattern
        static void Main(string[] args)
        {

        }

        static void PrintHelp()
        {
            Console.WriteLine(String.Format("usage: {0} <COST> <PAYED>", Environment.GetCommandLineArgs()[0]));
        }
    }
}
