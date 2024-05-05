using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ValleyReminders
{
    static class Utilities
    {
        internal static IModHelper Helper;

        internal static IMonitor Monitor;

        public static void SaveReminders(List<Reminder> reminder, string key)
        {
            Helper.Data.WriteSaveData(key, reminder);
        }

        public static List<Reminder> LoadReminders(string key)
        {
            List<Reminder>? reminders = Helper.Data.ReadSaveData<List<Reminder>>(key);
            return reminders ?? new(); //Create an empty list if save data does not contain the list
        }
    }
}
