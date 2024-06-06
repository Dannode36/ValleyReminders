using StardewModdingAPI.Utilities;
using StardewValley;
using System.Reflection;

//Represents a list of parameter names paired to their corresponding type
using ParameterList = System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, ValleyReminders.ParameterType>>;

namespace ValleyReminders
{
    //Used for parsing user input/UI
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

            try
            {
                string?[] pNames = GetParameterNames(methodName);
                ParameterType[] pTypes = GetParameterTypes(methodName);

                for (int i = 0; i < pNames.Length; i++)
                {
                    parameterList.Add(new(pNames[i] ?? string.Empty, pTypes[i]));
                }
            }
            catch (IndexOutOfRangeException)
            {
                Utilities.Monitor.Log("ParameterTypes/Defintion mismatch when creating parameter list. Please contact mod author", StardewModdingAPI.LogLevel.Error);
                throw;
            }

            return parameterList;
        }

        public static int GetParameterCount(string methodName)
        {
            return GetParameterNames(methodName).Length;
        }

        public static string?[] GetParameterNames(string methodName)
        {
            return typeof(Conditions).GetMethod(methodName)?.GetParameters()?.Select(x => x.Name).ToArray() ?? Array.Empty<string?>();
        }

        public static ParameterType[] GetParameterTypes(string methodName)
        {
            return typeof(Conditions)
                .GetMethod(methodName)?
                .GetCustomAttribute(typeof(ParameterTypeAttribute)) is not ParameterTypeAttribute attribute ? Array.Empty<ParameterType>() : attribute.ParamTypes;
        }

        //Date & Time
        [ParameterType(ParameterType.Int)]
        public static bool IsTimeOfDay(string time) => Game1.timeOfDay == int.Parse(time);

        [ParameterType(ParameterType.Int)]
        public static bool IsDayOfMonth(string day) => SDate.Now().Day == int.Parse(day);

        [ParameterType(ParameterType.Int)]
        public static bool IsDay(string day) => SDate.Now().DaysSinceStart == int.Parse(day);

        [ParameterType(ParameterType.String)]
        public static bool IsSeason(string season) => SDate.Now().SeasonKey == season;

        [ParameterType(ParameterType.Int)]
        public static bool IsYear(string year) => SDate.Now().Year == int.Parse(year);

        //World
        public static bool IsRaining() => Game1.isRaining;

        public static bool IsRainingTomorrow() => Game1.weatherForTomorrow == Game1.weather_rain;

        [ParameterType(ParameterType.String)]
        public static bool IsWeatherTomorrow(string weather) => Game1.weatherForTomorrow == weather;

        //Player
        [ParameterType(ParameterType.Int)]
        public static bool HasAchievement(string achievement) => Game1.achievements.ContainsKey(int.Parse(achievement));

        [ParameterType(ParameterType.Int, ParameterType.Int)]
        public static bool HasSkillLevel(string skill, string level) => Game1.player.GetSkillLevel(int.Parse(skill)) >= int.Parse(level);

        public static bool HasSkullKey() => Game1.player.hasSkullKey;
    }
}
