using StardewModdingAPI.Utilities;
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
        public static List<string> validCondFuncNames = new();

        public delegate bool Condition(List<string> args);

        //Date & Time
        public static bool IsTimeOfDay(List<string> args) => Game1.timeOfDay == int.Parse(args[0]);

        public static bool IsDayOfMonth(List<string> args) => SDate.Now().Day == int.Parse(args[0]);

        public static bool IsDay(List<string> args) => SDate.Now().DaysSinceStart == int.Parse(args[0]);

        public static bool IsSeason(List<string> args) => SDate.Now().SeasonKey == args[0];

        public static bool IsYear(List<string> args) => SDate.Now().Year == int.Parse(args[0]);

        //World
        public static bool IsRaining(List<string> args) => Game1.isRaining; 

        //Player
        public static bool HasAchievement(List<string> args) => Game1.achievements.ContainsKey(int.Parse(args[0]));
        
        public static bool HasSkillLevel(List<string> args) => Game1.player.GetSkillLevel(int.Parse(args[0])) >= int.Parse(args[1]);

        public static bool HasSkullKey(List<string> args) => Game1.player.hasSkullKey;
    }
}
