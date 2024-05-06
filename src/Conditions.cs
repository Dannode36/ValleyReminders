using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValleyReminders
{
    using Args = List<string>;

    static class Conditions
    {
        public delegate bool Condition(Args args);

        //Date & Time
        public static bool IsTimeOfDay(Args args) => Game1.timeOfDay == int.Parse(args[0]);

        public static bool IsDayOfMonth(Args args) => SDate.Now().Day == int.Parse(args[0]);

        public static bool IsDay(Args args) => SDate.Now().DaysSinceStart == int.Parse(args[0]);

        public static bool IsSeason(Args args) => SDate.Now().SeasonKey == args[0];

        public static bool IsYear(Args args) => SDate.Now().Year == int.Parse(args[0]);

        //World
        public static bool IsRaining(Args args) => Game1.isRaining; 

        //Player
        public static bool HasAchievement(Args args) => Game1.achievements.ContainsKey(int.Parse(args[0]));
        
        public static bool HasSkillLevel(Args args) => Game1.player.GetSkillLevel(int.Parse(args[0])) >= int.Parse(args[1]);

        public static bool HasSkullKey(Args args) => Game1.player.hasSkullKey;
    }
}
