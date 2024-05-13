using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using ValleyReminders.ui;

using Textbox = ValleyReminders.ui.Textbox;

namespace ValleyReminders
{
    enum ReminderMenuState
    {
        LIST,
        EDIT,
        CREATE
    }

    class ReminderMenu : IClickableMenu
    {
        private RootElement rootElement = new();
        private ReminderMenuState state = ReminderMenuState.LIST;

        private Table reminderListPage = new();
        private List<Reminder> reminders = new();
        bool reminderListDirty = false;

        private Table reminderEditPage = new();
        private Reminder? selectedReminder = null;

        private Table reminderCreationPage = new();

        public void CreateInterface(List<Reminder> reminders)
        {
            this.reminders = reminders;
            initialize((Game1.uiViewport.Width / 2) - 500, (Game1.uiViewport.Height / 2) - 500, 1000, 800, true);
            CreateStaticInterface();
            UpdateReminderList();

            rootElement.AddChild(reminderListPage);
            rootElement.AddChild(reminderEditPage);
            rootElement.AddChild(reminderCreationPage);
        }

        private void CreateStaticInterface()
        {
            rootElement = new();

            //Misc
            {
                /*var button = new Button(Utilities.Helper.ModContent.Load<Texture2D>(Utilities.Button))
                {
                    Callback = (e) => { reminders.Clear(); reminderListDirty = true; }
                };*/
                //rootElement.AddChild(button);

                /*var button = new Button(Game1.mouseCursors, new(384, 396, 15, 15), new(200, 50))
                {
                    Callback = (e) => { reminders.Add(new()); reminderListDirty = true; }
                };*/

                var label = new Label() { String = "Clear Reminders" };
                rootElement.AddChild(label);
            }
        }

        public void UpdateReminderList()
        {
            try
            {
                rootElement.RemoveChild(reminderListPage);
            }
            catch (ArgumentException) { /* Do nothing */ }

            //Reminders
            reminderListPage = new()
            {
                RowHeight = 100,
                Size = new(height, width),
                LocalPosition = new((Game1.uiViewport.Width / 2f) - 400, (Game1.uiViewport.Height / 2f) - 400)
            };

            foreach (Reminder reminder in reminders)
            {
                var button = new Button(Game1.mouseCursors, new(384, 396, 15, 15), new(200, 50))
                {
                    Callback = (e) => { selectedReminder = reminder; }
                };
                var textBox = new Label()
                {
                    String = reminder.Message,
                    Font = Game1.smallFont
                    //LocalPosition = new(Position.X, Position.Y)
                };
                reminderListPage.AddRow(new Element[] { button, textBox });
            }

            foreach (string funcName in Conditions.validCondFuncNames)
            {
                var button = new Button(Game1.mouseCursors, new(384, 396, 15, 15), new(200, 50))
                {
                    Callback = (e) => { }
                };
                var textBox = new Label()
                {
                    String = funcName,
                    Font = Game1.smallFont
                    //LocalPosition = new(Position.X, Position.Y)
                };
                reminderListPage.AddRow(new Element[] { button, textBox });
            }
        }

        public override void update(GameTime time)
        {
            base.update(time);

            if (reminderListDirty)
            {
                UpdateReminderList();
                reminderListDirty = false;
            }

            if(selectedReminder != null && state != ReminderMenuState.EDIT)
            {
                state = ReminderMenuState.EDIT;
                UpdateReminderEditPage();
            }

            switch (state)
            {
                case ReminderMenuState.LIST:
                    reminderListPage.Update();
                    break;
                case ReminderMenuState.EDIT:
                    reminderEditPage.Update();
                    break;
                case ReminderMenuState.CREATE:
                    reminderCreationPage.Update();
                    break;
                default:
                    break;
            }
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            reminderListPage.Scrollbar.ScrollBy(direction);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
        }

        public override void draw(SpriteBatch b)
        {
            switch (state)
            {
                case ReminderMenuState.LIST:
                    reminderListPage.Draw(b);
                    break;
                case ReminderMenuState.EDIT:
                    reminderEditPage.Draw(b);
                    break;
                case ReminderMenuState.CREATE:
                    reminderCreationPage.Draw(b);
                    break;
                default:
                    break;
            }

            if (shouldDrawCloseButton())
            {
                upperRightCloseButton.draw(b);
            }
            drawMouse(b);
        }
    }
}
