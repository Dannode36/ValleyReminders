using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace ValleyReminders
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        const string SAVE_KEY = "vr-reminderList";

        public List<Reminder> activeReminders = new()
        {
            new("This is a reccuring alarm for 6:10am", 0, 610),
            new("This is a reflection strainer", 0, 620, 1, new()
            {
                new("IsDay", "8"),
                new("IsSeason", "Spring"),
                new("IsYear", "1"),
            }),
            new("This is a reflection strainer 2", 0, 630, 1, new()
            {
                new("IsDay", "8"),
                new("IsSeason", "Spring"),
                new("IsYear", "1"),
            }),
            new("This is a reflection strainer 3", 0, 640, 1, new()
            {
                new("IsDay", "8"),
                new("IsSeason", "Spring"),
                new("IsYear", "1"),
            })
        };

        readonly List<Reminder> deleteQueue = new();

        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            Utilities.Helper = helper;
            Utilities.Monitor = Monitor;

            helper.Events.GameLoop.TimeChanged += TimeChangedHandler;
            helper.Events.GameLoop.DayStarted += DayStartedHandler;
            helper.Events.GameLoop.Saving += SavingHandler;
            helper.Events.GameLoop.SaveLoaded += SaveLoadedHandler;
        }

        /*********
        ** Private methods
        *********/

        private void SaveLoadedHandler(object? sender, SaveLoadedEventArgs e)
        {
            activeReminders = Utilities.LoadReminders(SAVE_KEY);
            Monitor.Log("Reminders loaded!", LogLevel.Info);
        }

        private void SavingHandler(object? sender, SavingEventArgs e)
        {
            Monitor.Log($"Removing {deleteQueue.Count} expired reminders", LogLevel.Info);
            //Delete expired reminders before writing to save
            foreach (var expiredReminder in deleteQueue)
            {
                activeReminders.Remove(expiredReminder);
            }

            Utilities.SaveReminders(activeReminders, SAVE_KEY);
            Monitor.Log("Reminders saved!", LogLevel.Info);
        }

        private void DayStartedHandler(object? sender, DayStartedEventArgs e)
        {
            ReminderLoop();
        }

        private void TimeChangedHandler(object? sender, TimeChangedEventArgs e)
        {
            ReminderLoop();
        }

        private void ReminderLoop()
        {
            //Update and notify reminders
            foreach (var reminder in activeReminders)
            {
                if (reminder.IsReadyToNotify())
                {
                    reminder.Notify();
                    Monitor.Log($"Reminder notified: {reminder.Message}", LogLevel.Info);

                    if (!reminder.IsRecurring())
                    {
                        deleteQueue.Add(reminder);
                    }
                }
            }
        }
    }
}
