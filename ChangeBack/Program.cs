using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Text.RegularExpressions;

using CurrencyAmount = System.Collections.Generic.Dictionary<ChangeBack.Currency, int>;

// TODO: Actually implement the Regex parser to get the cost and paid amounts, calculate the change, and figure out the coins to give back.





namespace ChangeBack
{
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
    };


    class Program
    {
        /// <summary>
        /// Any amount of comma-separated digits, followed by an optional pair 
        /// of decimal point and TWO decimals (we don't care about pesky
        /// hundredths-of-a-cent!)
        /// </summary>
        private static Regex CurrencyPattern = new Regex(@"(?<dollars>[\d,]+)\.(?<cents>\d{0,2})");

        private static Currency[] CURRENCY_SORT_ORDER = { Currency.Dollar, Currency.Quarter, Currency.Dime, Currency.Nickel, Currency.Penny };

        static void Main(string[] args)
        {

            PrintHelp();

                Console.WriteLine("Starting Main");
                CurrencyAmount cost = GetCurrencyAmountFromString(args[0]);

                Console.WriteLine("Parsed cost, begin Parse paid");
                CurrencyAmount paid = GetCurrencyAmountFromString(args[1]);

                Console.WriteLine("Parsed paid, begin calculate change");
                CurrencyAmount change = SubtractCurrencyAmounts(paid, cost);

                Console.WriteLine("Calculated change, begin print change");

                Console.WriteLine("Change: {0}", GetStringFromCurrencyAmount(change));

            Console.ReadLine();

        }

        static CurrencyAmount GetCurrencyAmountFromString(string currStr)
        {
            CurrencyAmount currAmt = new CurrencyAmount();
            Match match = CurrencyPattern.Match(currStr);

            string dollarString = match.Groups["dollars"].Value;
            string centsString = match.Groups["cents"].Value;

            currAmt[Currency.Dollar] = GetDollarsFromString(dollarString);

            int cents = GetCentsFromString(currStr);

            // Loop through all currency denominations from largest to smallest
            // and, if there is enough money to be converted into the
            // denomination, convert it.
            // By going from largest first to smallest last, we use the fewest
            // number of currency.
            for (int i = 0; i < (CURRENCY_SORT_ORDER.Length - 1); i++)
            {
                // Converted to byte here because Currency is represented
                // internally as bytes.
                while (cents > (int)CURRENCY_SORT_ORDER[i])
                {
                    currAmt[CURRENCY_SORT_ORDER[i]] += 1;
                    cents -= (int)CURRENCY_SORT_ORDER[i];
                }

                // Make sure to set every denomination, even if it is zero (0).
                if (!currAmt.ContainsKey(CURRENCY_SORT_ORDER[i]))
                {
                    currAmt[CURRENCY_SORT_ORDER[i]] = 0;
                }
            }

            return currAmt;
        }

        static CurrencyAmount GetCurrencyAmountFromInt(int c)
        {
            CurrencyAmount currAmt = new CurrencyAmount();

            for (int i = 0; i < CURRENCY_SORT_ORDER.Length - 1; i++)
            {
                while (c > (int)CURRENCY_SORT_ORDER[i])
                {
                    int oldVal = 0;
                    currAmt.TryGetValue(CURRENCY_SORT_ORDER[i], out oldVal);
                    currAmt[CURRENCY_SORT_ORDER[i]] = oldVal + 1;
                    c -= (int)CURRENCY_SORT_ORDER[i];
                }

                // Make sure to set every denomination, even if it is zero (0).
                if (!currAmt.ContainsKey(CURRENCY_SORT_ORDER[i]))
                {
                    currAmt[CURRENCY_SORT_ORDER[i]] = 0;
                }
            }

            return currAmt;
        }

        static int GetDollarsFromString(string dollarString)
        {
            // Match match = CurrencyPattern.Match(currStr);

            int dollars = int.Parse(dollarString);

            return dollars;
        }

        static int GetCentsFromString(string currStr)
        {
            Match match = CurrencyPattern.Match(currStr);
            int cents = 0;

            try
            {
                cents = int.Parse(match.Captures[1].ToString());
            }
            catch (ArgumentOutOfRangeException)
            {
                cents = 0;
            }

            return cents;
        }

        /*
        static int GetCurrencyAmountFromInt(CurrencyAmount c)
        {
            int total = 0;
            for (int i = 0; i < CURRENCY_SORT_ORDER.Length - 1; i++)
            {
                total += c[CURRENCY_SORT_ORDER[i]] * (int)CURRENCY_SORT_ORDER[i];
            }

            return total;
        }
        */

        static int GetIntFromCurrencyAmount(CurrencyAmount c)
        {
            int total = 0;
            for (int i = 0; i < (CURRENCY_SORT_ORDER.Length - 1); i++)
            {
                // Turn the mapped count of each denomination into an integer 
                // representing the number of pennies it is equivalent to.
                total += c[CURRENCY_SORT_ORDER[i]] * (int)CURRENCY_SORT_ORDER[i];
            }

            return total;
        }

        static CurrencyAmount SubtractCurrencyAmounts(CurrencyAmount a, CurrencyAmount b)
        {
            int amtA = GetIntFromCurrencyAmount(a);
            int amtB = GetIntFromCurrencyAmount(b);
            int diff = amtA - amtB;

            return GetCurrencyAmountFromInt(diff);
        }

        static string GetStringFromCurrencyAmount(CurrencyAmount c)
        {
            return String.Format("[Total: {0}]:\n\tDollars: {1}\n\tQuarters: {2}\n\tDimes: {3}\n\tNickels: {4}\n\tPennies: {5}", 
                GetIntFromCurrencyAmount(c),
                (byte)c[Currency.Dollar],
                (byte)c[Currency.Quarter],
                (byte)c[Currency.Dime],
                (byte)c[Currency.Nickel],
                (byte)c[Currency.Penny]);
        }

        static void PrintHelp()
        {
            Console.WriteLine(String.Format("usage: {0} <COST> <PAYED>", Environment.GetCommandLineArgs()[0]));
        }
    }
}
