using JuanMartin.Kernel.RuleEngine;
using System;

namespace JuanMartin.Kernel.Utilities
{
    public enum DaysOfWeek { Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday };
    public enum MonthsOfYear { Jan, Feb, Mar, Apr, May, Jun, Jul, Aug, Sep, Oct, Nov, Dec };

    public class UtilityDate
    {
        [Macro]
        public static Value Now()
        {
            return new Value(DateTime.Now);
        }

        [Macro]
        public static Value IsWeekend(Symbol Operand)
        {
            Value result = new Value();

            DateTime dtm = (DateTime)Operand.Value.Result;

            result.Result = ((int)dtm.DayOfWeek == 5 || (int)dtm.DayOfWeek == 6);

            return result;
        }

        public static bool IsLeapYear(DateTime date)
        {
            var year = date.Year;

            return (year % 4 == 0) && (year % 400 != 0);
        }
    }
}
