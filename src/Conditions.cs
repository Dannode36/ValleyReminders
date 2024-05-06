using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValleyReminders
{
    static class Conditions
    {
        public static bool IsDayOfMonth(List<string> args)
        {
            return Game1.dayOfMonth == int.Parse(args[0]);
        }

        public static bool IsDay(List<string> args)
        {
            return Game1.Date.TotalDays == int.Parse(args[0]);
        }
    }
}
