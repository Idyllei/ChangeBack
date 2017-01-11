using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions; // For parsing input to penny amounts.

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
        private static string CurrencySymbol = "";
        /// <summary>
        /// Number separated by thousands with period or comma, with 2 optional decimal places, with the currency symbol either before or after.
        /// </summary>
        private Regex CURRENCY_PATTERN = new Regex("(" + CurrencySymbol + ")?\s*\d[,\.]?(\d{,3}[\.,])?([\.,]\d{0,2})?" + "(" + CurrencySymbol + ")?");
        static void Main(string[] args)
        {
            GetCurrencySymbol();
        }

        static void GetCurrencySymbol()
        {
            Console.Write("What symbol do you use before (or after) your currency?\t");
            CurrencySymbol = Console.ReadLine().Trim();
        }

        static void PrintHelp()
        {
            Console.WriteLine("usage: ");
        }
    }
}
