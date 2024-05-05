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
    class Reminder : IReminder
    {
        public string Message { get; set; } = string.Empty;
        public int StartDay { get; set; } = 0;
        public int Time { get; set; } = 600; //Only valid in increments of 10 in the range 600-2600
        public int Interval { get; set; } = -1;

        public Reminder() { }

        public Reminder(string message, int startDay, int interval = -1, int time = 600)
        {
            Message = message;
            StartDay = startDay;
            Time = time;
            Interval = interval;
        }

        public bool IsReadyToNotify()
        {
            if(Interval != -1) //Periodic reminder
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
            else // One-shot reminder
            {
                return SDate.Now().DaysSinceStart == StartDay;
            }
        }

        public bool IsRecurring()
        {
            return Interval != -1;
        }

        public void Notify()
        {
            Game1.addHUDMessage(new HUDMessage(Message, HUDMessage.newQuest_type));
        }
    }
}
