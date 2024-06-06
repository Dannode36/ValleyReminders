using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

using ParameterList = System.Collections.Generic.List<System.Collections.Generic.KeyValuePair<string, ValleyReminders.ParameterType>>;

namespace ValleyReminders
{
    /// <summary>The mod entry point.</summary>
    internal sealed class ModEntry : Mod
    {
        const string SAVE_KEY = "vr-reminderList";
        private ModConfig Config = new();
        private readonly ReminderMenu menu = new();

        public List<Reminder> activeReminders = new()
        {
            new("This is a reccuring alarm for 6:10am", 0, 610),
            new("This is a reflection strainer", 0, 620, 1, new()
            {
                new("IsDay", new(){ "8" }),
                new("IsSeason", new(){ "Spring" }),
                new("IsYear", new() { "1" }),
            }),
            new("This is a reflection strainer 2", 0, 630, 1, new()
            {
                new("IsDay", new() { "8" }),
                new("IsSeason", new() { "Spring" }),
                new("IsYear", new() { "1" }),
            }),
            new("This is a reflection strainer 3", 0, 640, 1, new()
            {
                new("IsDay", new() { "8" }),
                new("IsSeason", new() { "Spring" }),
                new("IsYear", new() { "1" }),
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
            Config = helper.ReadConfig<ModConfig>();
            Utilities.Helper = helper;
            Utilities.Monitor = Monitor;

            //Get the names of all reminder condition functions available to the user at runtime
            {
                List<string> methodNameList = typeof(Conditions)
                .GetMethods()
                .Where(x => x.ReturnType == typeof(bool) && x.IsStatic)
                .Select(x => x.Name)
                .ToList();

                SortedDictionary<string, ParameterList> methodNameAndParameterInfo = new();

                foreach (var methodName in methodNameList)
                {
                    methodNameAndParameterInfo.Add(methodName, Conditions.GetParameterList(methodName));
                }

                Conditions.conditionFunctions = methodNameAndParameterInfo;
            }


            helper.Events.GameLoop.TimeChanged += OnTimeChanged;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
            helper.Events.GameLoop.Saving += OnSaving;
            helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicker;

            helper.Events.Display.RenderedActiveMenu += OnRenderedActiveMenu;

            helper.Events.Input.ButtonsChanged += OnButtonsChanged;
            helper.Events.Input.MouseWheelScrolled += OnMouseWheelScrolled;
        }

        /*********
        ** Private methods
        *********/

        private void OnUpdateTicker(object? sender, UpdateTickedEventArgs e)
        {
            if (Game1.hasLoadedGame && Game1.activeClickableMenu is ReminderMenu menu)
            {
                menu.update(Game1.currentGameTime);
            }
        }

        private void OnButtonsChanged(object? sender, ButtonsChangedEventArgs e)
        {
            if (Game1.hasLoadedGame && Config.ToggleKey.JustPressed())
            {
                Game1.activeClickableMenu = menu;
            }
        }

        private void OnMouseWheelScrolled(object? sender, MouseWheelScrolledEventArgs e)
        {
            if (Game1.hasLoadedGame && Game1.activeClickableMenu is ReminderMenu menu)
            {
                menu.receiveScrollWheelAction(e.Delta);
            }
        }

        private void OnRenderedActiveMenu(object? sender, RenderedActiveMenuEventArgs e)
        {
            if(Game1.activeClickableMenu is ReminderMenu menu)
            {
                menu.draw(e.SpriteBatch);
            }
        }

        private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
        {
            //activeReminders = Utilities.LoadReminders(SAVE_KEY);
            Monitor.Log("Reminders loaded!");

            menu.CreateInterface(activeReminders);
        }

        private void OnSaving(object? sender, SavingEventArgs e)
        {
            Monitor.Log($"Removing {deleteQueue.Count} expired reminders");
            //Delete expired reminders before writing to save
            foreach (var expiredReminder in deleteQueue)
            {
                activeReminders.Remove(expiredReminder);
            }

            Utilities.SaveReminders(activeReminders, SAVE_KEY);
            Monitor.Log("Reminders saved!");
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            ReminderLoop();
        }

        private void OnTimeChanged(object? sender, TimeChangedEventArgs e)
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
                    Monitor.Log($"Reminder notified: {reminder.Message}");

                    if (!reminder.IsRecurring())
                    {
                        deleteQueue.Add(reminder);
                    }
                }
            }
        }
    }
}
