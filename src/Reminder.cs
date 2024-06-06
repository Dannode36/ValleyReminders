using Microsoft.Xna.Framework;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValleyReminders
{
    class Reminder
    {
        public string Message { get; set; } = string.Empty;
        public int StartDay { get; set; }
        public int Time { get; set; } //Only valid in increments of 10 in the range 600-2600
        public int Interval { get; set; }
        public bool Enabled { get; set; } = true;

        public List<ConditionUserData> Conditions = new();

        public Reminder() { }

        public Reminder(string message, int startDay, int time = 600, int interval = -1, List<ConditionUserData>? conditions = null)
        {
            Message = message;
            StartDay = startDay;
            Time = time;
            Interval = interval;
            Conditions = conditions ?? new();
        }

        public bool IsReadyToNotify()
        {
            if(!Enabled) return false;

            foreach (var condition in Conditions)
            {
                object?[] objects = condition.ParameterValues.ToArray();
                object? result = typeof(Conditions).GetMethod(condition.MethodName)?.Invoke(this, objects);
                if ((bool?)result == false) return false;
            }

            if (SDate.Now().DaysSinceStart >= StartDay)
            {
                if (IsRecurring()) //Recurring reminder
                {
                    if ((SDate.Now().DaysSinceStart - StartDay) % Interval == 0)
                    {
                        return Game1.timeOfDay == Time;
                    }
                    else
                    {
                        return false;
                    }
                }
                else // One-shot reminder
                {
                    return Game1.timeOfDay == Time;
                }
            }
            else return false;
        }

        public bool IsRecurring()
        {
            return Interval > 0;
        }

        public void Notify()
        {
            Game1.addHUDMessage(new HUDMessage(Message, HUDMessage.newQuest_type));
            Game1.playSound("detector");
        }
    }
}
