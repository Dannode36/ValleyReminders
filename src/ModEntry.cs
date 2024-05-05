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

        public List<Reminder> activeReminders = new();

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
        }

        /*********
        ** Private methods
        *********/

        private void SavingHandler(object? sender, SavingEventArgs e)
        {
            //Delete expired reminders before writing to save
            foreach (var expiredReminder in deleteQueue)
            {
                activeReminders.Remove(expiredReminder);
            }

            Utilities.SaveReminders(activeReminders, SAVE_KEY);
            Monitor.Log("Reminders saved", LogLevel.Info);
        }

        private void DayStartedHandler(object? sender, DayStartedEventArgs e)
        {
            activeReminders = Utilities.LoadReminders(SAVE_KEY);
            Monitor.Log("Reminders loaded", LogLevel.Info);
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

                    if (reminder.IsRecurring())
                    {
                        Monitor.Log($"Recurring reminder notified for day {reminder.StartDay}: {reminder.Message}", LogLevel.Info);
                    }
                    else
                    {
                        Monitor.Log($"Reminder notified for day {reminder.StartDay}: {reminder.Message}", LogLevel.Info);
                        deleteQueue.Add(reminder);
                    }
                }
            }
        }
    }
}
