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
        /// <summary>
        /// Any amount of comma-separated digits, followed by an optional pair 
        /// of decimal point and TWO decimals (we don't care about pesky
        /// hundredths-of-a-cent!)
        /// </summary>
        private static Regex CurrencyPattern = new Regex(@"\$([\d,]+(?\.\d{0,2})?)");

        private static Currency[] CURRENCY_SORT_ORDER = { Currency.Dollar, Currency.Quarter, Currency.Dime, Currency.Nickel, Currency.Penny };

        static void Main(string[] args)
        {

        }

        static Dictionary<Currency, int> GetCurrencyAmountFromString(string currStr)
        {
            Dictionary<Currency, int> currAmt = new Dictionary<Currency, int>();
            Match match = CurrencyPattern.Match(currStr);

            currAmt[Currency.Dollar] = GetDollarsFromString(currStr);

            int cents = GetCentsFromString(currStr);

            // Loop through all currency denominations from largest to smallest
            // and, if there is enough money to be converted into the
            // denomination, convert it.
            // By going from largest first to smallest last, we use the fewest
            // number of currency.
            for (int i = 0; i < CURRENCY_SORT_ORDER.Length - 1; i++)
            {
                // Converted to byte here because Currency is represented
                // internally as bytes.
                while (cents > (byte)CURRENCY_SORT_ORDER[i])
                {
                    currAmt[CURRENCY_SORT_ORDER[i]] += 1;
                    cents -= (byte)CURRENCY_SORT_ORDER[i];
                }
            }

            return currAmt;
        }

        static int GetDollarsFromString(string currStr)
        {
            Match match = CurrencyPattern.Match(currStr);

            int dollars = (int)Math.Floor(float.Parse(match.Captures[0].ToString()));

            return dollars;
        }

        static int GetCentsFromString(string currStr)
        {
            Match match = CurrencyPattern.Match(currStr);

            int cents = (int)(float.Parse(match.Captures[0].ToString()) - GetDollarsFromString(currStr));

            return cents;
        }

        static void PrintHelp()
        {
            Console.WriteLine(String.Format("usage: {0} <COST> <PAYED>", Environment.GetCommandLineArgs()[0]));
        }
    }
}
