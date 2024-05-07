using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
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

        readonly RootElement rootElement = new();

        public void CreateInterface(List<Reminder> reminders)
        {
            Table table = new()
            {
                RowHeight = 100,
                Size = new(1000, 1000),
                LocalPosition = new(Game1.viewportCenter.X, Game1.viewportCenter.Y)
            };
            List<Element> row1 = new()
            {
                new Checkbox(),
                new Label() { String = "Reminders Enabled", LocalPosition = new (new Checkbox().Width + 10, 0)}
            };
            table.AddRow(row1.ToArray());

            //Reminders
            Table reminderTable = new(false)
            {
                RowHeight = 20,
                Size = new(800, 500),
                LocalPosition = new (Position.X + 100, Position.Y)
            };
            foreach (Reminder reminder in reminders)
            {
                List<Element> reminderData = new()
                {
                    new Textbox()
                    {
                        String = reminder.Message,
                        LocalPosition = new(Position.X , Position.Y)
                    }
                };
                reminderTable.AddRow(reminderData.ToArray());
            }

            List<Element> row2 = new()
            {
                reminderTable
            };
            table.AddRow(row2.ToArray());

            rootElement.AddChild(table);
        }

        public override void update(GameTime time)
        {
            base.update(time);
            rootElement.Update();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
        }

        public override void draw(SpriteBatch b)
        {
            rootElement.Draw(b);
            drawMouse(b);
        }
    }
}
