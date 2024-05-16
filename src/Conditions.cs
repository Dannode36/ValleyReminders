using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ValleyReminders.src;

namespace ValleyReminders
{
    struct Condition
    {
        public readonly string[] p_names;
        public readonly Func<string[], bool> cond;

        public Condition(string[] p_names, Func<string[], bool> cond)
        {
            this.p_names = p_names;
            this.cond = cond;
        }
    }

    static class Conditions
    {
        public static List<string> validCondFuncNames = new();

        //Date & Time
        [ParameterNames("Time")]
        public static bool IsTimeOfDay(List<string> args) => Game1.timeOfDay == int.Parse(args[0]);

        [ParameterNames("Day")]
        public static bool IsDayOfMonth(List<string> args) => SDate.Now().Day == int.Parse(args[0]);

        [ParameterNames("Day")]
        public static bool IsDay(List<string> args) => SDate.Now().DaysSinceStart == int.Parse(args[0]);

        [ParameterNames("Season")]
        public static bool IsSeason(List<string> args) => SDate.Now().SeasonKey == args[0];

        [ParameterNames("Year")]
        public static bool IsYear(List<string> args) => SDate.Now().Year == int.Parse(args[0]);

        //World
        [ParameterNames]
        public static bool IsRaining(List<string> args) => Game1.isRaining;

        [ParameterNames]
        public static bool IsRainingTomorrow(List<string> args) => Game1.weatherForTomorrow == Game1.weather_rain;

        [ParameterNames("Weather type")]
        public static bool IsWeatherTomorrow(List<string> args) => Game1.weatherForTomorrow == args[0];

        //Player
        [ParameterNames("Achievement")]
        public static bool HasAchievement(List<string> args) => Game1.achievements.ContainsKey(int.Parse(args[0]));
        
        [ParameterNames("Skill", "Level")]
        public static bool HasSkillLevel(List<string> args) => Game1.player.GetSkillLevel(int.Parse(args[0])) >= int.Parse(args[1]);

        [ParameterNames]
        public static bool HasSkullKey(List<string> args) => Game1.player.hasSkullKey;
    }
}
