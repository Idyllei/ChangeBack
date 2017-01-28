using System;

using System.Text.RegularExpressions;



// TODO: make the currency digit separator customizable. It is currently hardcoded to be a comma (,).
// TODO: make the currency symbol customizable. It is currently hardcoded to be a dollar sign ($).



namespace ChangeBack
{
    using CurrencyAmount = System.Collections.Generic.Dictionary<Currency, int>;


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
        private static RegexOptions BetaCurrPttn2Opts = RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.ExplicitCapture;
        private static Regex BetaCurrPttn2 = new Regex(@"
                                                        (?<dollarSign>\$\s*)                # We will allow a dollar sign at the beginning or the end. Check at front.
                                                        (?(dollarSign)                      # IF dollar sign at beginning
                                                            (?<money>
                                                                (?<dollars>(\d*,?)*)  # Allow digits with a comma separating them
                                                                \.?                         # Allow a decimal place
                                                                (?<cents>\d*)               # Allow any number of decimal places
                                                            )                               # END ?<money>
                                                            |                               # ELSE
                                                            (
                                                                \k<money>                   # reuse <money> pattern
                                                                \s*?
                                                                \k<dollarSign>?             # Allow dollar sign at end if not at front
                                                            )                               # END ?(dollarSign)
                                                         )
                                                        ",
                                                        BetaCurrPttn2Opts);

        private static Currency[] CURRENCY_SORT_ORDER = { Currency.Dollar, Currency.Quarter, Currency.Dime, Currency.Nickel, Currency.Penny };


        static void Main(string[] args)
        {

            PrintHelp();

            // Console.WriteLine("Starting Main");
            CurrencyAmount cost = GetCurrencyAmountFromString(args[0]);

            // Console.WriteLine(String.Format("Cost: {0}", GetStringFromCurrencyAmount(cost)));
                
            // Console.WriteLine("Parsed cost, begin Parse paid");
            CurrencyAmount paid = GetCurrencyAmountFromString(args[1]);

            // Console.WriteLine(String.Format("Paid: {0}", GetStringFromCurrencyAmount(paid)));

            // Console.WriteLine("Parsed paid, begin calculate change");
            CurrencyAmount change = SubtractCurrencyAmounts(paid, cost);

            // Console.WriteLine("Calculated change, begin print change");

            Console.WriteLine("Change: {0}", GetStringFromCurrencyAmount(change));

            Console.ReadLine();

        }


        static CurrencyAmount GetCurrencyAmountFromString(string currStr)
        {
            CurrencyAmount currAmt = new CurrencyAmount();
            Match match = BetaCurrPttn2.Match(currStr);


            currAmt[Currency.Dollar] = GetDollarsFromMatch(match);

            int cents = GetCentsFromMatch(match);

            // Loop through all currency denominations from largest to smallest
            // and, if there is enough money to be converted into the
            // denomination, convert it.
            // By going from largest first to smallest last, we use the fewest
            // number of currency.
            foreach (Currency denom in CURRENCY_SORT_ORDER)
            {
                // Converted to byte here because Currency is represented
                // internally as bytes.
                while (cents >= (int)denom)
                {
                    int oldVal = 0;
                    currAmt.TryGetValue(denom, out oldVal);
                    currAmt[denom] = oldVal + 1;

                    cents -= (int)denom;
                }

                // Make sure to set every denomination.
                if (!(currAmt.ContainsKey(denom)))
                {
                    currAmt[denom] = 0;
                }
            }

            return currAmt;
        }


        static CurrencyAmount GetCurrencyAmountFromInt(int c)
        {
            CurrencyAmount currAmt = new CurrencyAmount();

            foreach (Currency denom in CURRENCY_SORT_ORDER)
            {
                while (c >= (int)denom)
                {
                    int oldVal = 0;
                    currAmt.TryGetValue(denom, out oldVal);
                    currAmt[denom] = oldVal + 1;
                    c -= (int)denom;
                }

                // Make sure to set every denomination, even if it is zero (0).
                if (!currAmt.ContainsKey(denom))
                {
                    currAmt[denom] = 0;
                }
            }

            return currAmt;
        }


        static int GetDollarsFromMatch(Match match)
        {
            // Match match = CurrencyPattern.Match(currStr);

            int dollars = 0;
            try
            {
                string dollarString = match.Groups["dollars"].Value.ToString();

                // Remove all separators (currently commas) from the string by replacing with empty strings.
                /// TODO: instead of harcoding a comma as separator, make it customizable
                string prunedDollarString = dollarString.Replace(",", "");

                dollars = int.Parse(prunedDollarString);
            }
            catch
            {
                dollars = 0;
            }

            return dollars;
        }


        static int GetCentsFromMatch(Match match)
        {
            int cents = 0;

            try
            {
                // centsString doesn't use a separator, so we don't need to prune it like a dollarString.
                string centsString = match.Groups["cents"].Value.ToString();

                cents = int.Parse(centsString);
            }
            catch
            {
                cents = 0;
            }

            return cents;
        }


        static int GetIntFromCurrencyAmount(CurrencyAmount c)
        {
            int total = 0;
            foreach (Currency denom in CURRENCY_SORT_ORDER)
            {
                int value = 0;
                c.TryGetValue(denom, out value);
                // Turn the mapped count of each denomination into an integer 
                // representing the number of pennies it is equivalent to.
                total += value * (int)denom;
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

        static decimal GetDecimalFromCurrencyAmount(CurrencyAmount c)
        {
            return (decimal)GetIntFromCurrencyAmount(c) / (byte)Currency.Dollar;
        }

        static string GetStringFromCurrencyAmount(CurrencyAmount c)
        {
            return String.Format("[Total: {0:c}]:\n\tDollars: {1}\n\tQuarters: {2}\n\tDimes: {3}\n\tNickels: {4}\n\tPennies: {5}", 
                GetDecimalFromCurrencyAmount(c),
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
