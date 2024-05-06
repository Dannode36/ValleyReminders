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
    class RecurringReminder : IReminder
    {
        public string Message { get; set; } = string.Empty;
        public int StartDay { get; set; } = 0;
        public int Time { get; set; } = 600; //Only valid in increments of 10 in the range 600-2600
        public int Interval { get; set; } = int.MaxValue; //defualts to only occur "once"

        public RecurringReminder() { }

        public RecurringReminder(string message, int startDay, int interval = -1, int time = 600)
        {
            Message = message;
            StartDay = startDay;
            Time = time;
            Interval = interval;
        }

        public bool IsReadyToNotify()
        {
            if ((SDate.Now().DaysSinceStart >= StartDay) && (SDate.Now().DaysSinceStart - StartDay) % Interval == 0)
            {
                return Game1.timeOfDay == Time;
            }
            else
            {
                return false;
            }
        }

        public bool IsRecurring()
        {
            return true;
        }

        public void Notify()
        {
            Game1.addHUDMessage(new HUDMessage(Message, HUDMessage.newQuest_type));
        }
    }
}
