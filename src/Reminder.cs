﻿using Microsoft.Xna.Framework;
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

        public Reminder() { }

        public Reminder(string message, int startDay, int time = 600)
        {
            Message = message;
            StartDay = startDay;
            Time = time;
        }

        public bool IsReadyToNotify()
        {
            return SDate.Now().DaysSinceStart == StartDay;
        }

        public bool IsRecurring()
        {
            return false;
        }

        public void Notify()
        {
            Game1.addHUDMessage(new HUDMessage(Message, HUDMessage.newQuest_type));
        }
    }
}
