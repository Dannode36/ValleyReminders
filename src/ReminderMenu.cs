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
    class ReminderMenu : IClickableMenu
    {
        private List<Reminder> reminders = new();

        private RootElement rootElement = new();
        private Table table = new();
        bool reminderListDirty;

        public void CreateInterface(List<Reminder> reminders)
        {
            this.reminders = reminders;
            initialize((Game1.uiViewport.Width / 2) - 500, (Game1.uiViewport.Height / 2) - 500, 1000, 800, true);
            CreateStaticInterface();
            UpdateReminderList();
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

                var label = new Label() { String = "Clear Reminders" };
                rootElement.AddChild(label);
            }
        }

        public void UpdateReminderList()
        {
            try
            {
                rootElement.RemoveChild(table);
            }
            catch (ArgumentException) { /* Do nothing */ }

            //Reminders
            table = new()
            {
                RowHeight = 100,
                Size = new(height, width),
                LocalPosition = new((Game1.uiViewport.Width / 2f) - 400, (Game1.uiViewport.Height / 2f) - 400)
            };

            foreach (Reminder reminder in reminders)
            {
                var textBox = new Label()
                {
                    String = reminder.Message,
                    Font = Game1.smallFont
                    //LocalPosition = new(Position.X, Position.Y)
                };
                table.AddRow(new Element[] { textBox });
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
                table.AddRow(new Element[] { button, textBox });
            }

            rootElement.AddChild(table);
        }

        public override void update(GameTime time)
        {
            if (reminderListDirty)
            {
                UpdateReminderList();
                reminderListDirty = false;
            }

            base.update(time);
            rootElement.Update();
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            table.Scrollbar.ScrollBy(direction);
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
        }

        public override void draw(SpriteBatch b)
        {
            rootElement.Draw(b);

            if (shouldDrawCloseButton())
            {
                upperRightCloseButton.draw(b);
            }
            drawMouse(b);
        }
    }
}
