using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

//Represents a list of parameter names paired to their corresponding type
using ParameterList = System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, ValleyReminders.ParameterType>>;

namespace ValleyReminders
{
    public enum ParameterType
    {
        String,
        Int,
        Float
    }

    static class Conditions
    {
        public static SortedDictionary<string, ParameterList> conditionFunctions = new();

        public static ParameterList GetParameterList(string methodName)
        {
            ParameterList parameterList = new();

            ParameterInfo[]? pInfos = typeof(Conditions).GetMethod(methodName)?.GetParameters();
            for (int i = 0; i < pInfos?.Length; i++)
            {
                parameterList.Add(new(pInfos[i].Name ?? string.Empty, GetParameterType(pInfos[i].ParameterType)));
            }

            return parameterList;
        }

        public static ParameterType GetParameterType(Type type)
        {
            if (type == typeof(int)) return ParameterType.Int;
            else if (type == typeof(float)) return ParameterType.Float;
            else return ParameterType.String;
        }

        //Date & Time
        public static bool IsTimeOfDay(int time) => Game1.timeOfDay == time;
        public static bool IsDayOfMonth(int day) => SDate.Now().Day == day;
        public static bool IsDay(int day) => SDate.Now().DaysSinceStart == day;
        public static bool IsSeason(string season) => SDate.Now().SeasonKey == season;
        public static bool IsYear(int year) => SDate.Now().Year == year;

        //World
        public static bool IsRaining() => Game1.isRaining;
        public static bool IsRainingTomorrow() => Game1.weatherForTomorrow == Game1.weather_rain;
        public static bool IsWeatherTomorrow(string weather) => Game1.weatherForTomorrow == weather;

        //Player
        public static bool HasAchievement(int achievement) => Game1.achievements.ContainsKey(achievement);
        public static bool HasSkillLevel(int skill, int level) => Game1.player.GetSkillLevel(skill) >= level;
        public static bool HasSkullKey() => Game1.player.hasSkullKey;
    }
}
