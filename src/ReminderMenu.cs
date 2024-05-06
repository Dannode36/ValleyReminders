using Microsoft.VisualBasic.FileIO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ValleyReminders.ui;

using Textbox = ValleyReminders.ui.Textbox;

namespace ValleyReminders
{
    class ReminderMenu : IClickableMenu
    {
        private List<Reminder> reminders = new();

        private List<ClickableComponent> labels = new List<ClickableComponent>();

        /// <summary>The season buttons to draw.</summary>
        private List<ClickableTextureComponent> seasonButtons = new List<ClickableTextureComponent>();

        /// <summary>The day buttons to draw.</summary>
        private List<ClickableTextureComponent> dayButtons = new List<ClickableTextureComponent>();

        OptionsCheckbox option = new("Enable Reminders", 10);

        readonly RootElement rootElement = new();

        public void CreateInterface(List<Reminder> reminders)
        {
            Table table = new()
            {
                RowHeight = 100,
                Size = new(1000, 1000)
            };
            List<Element> row1 = new()
            {
                new Checkbox(),
                new Label() { String = "Reminders Enabled", LocalPosition = new (20, 0)}
            };
            table.AddRow(row1.ToArray());

            //Reminders
            Table reminderTable = new(false)
            {
                RowHeight = 20,
                Size = new(800, 500),
                LocalPosition = new (20, 0)
            };
            foreach (Reminder reminder in reminders)
            {
                List<Element> reminderData = new()
                {
                    new Textbox() { String = reminder.Message, }
                };
                reminderTable.AddRow(reminderData.ToArray());
            }

            List<Element> row2 = new()
            {
                reminderTable
            };
            table.AddRow(row2.ToArray());

            rootElement.AddChild(table);
            rootElement.LocalPosition = new(Game1.viewportCenter.X, Game1.viewportCenter.Y);
        }

        public override void update(GameTime time)
        {
            base.update(time);
            rootElement.Update();
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);

            if (option.bounds.Contains(x, y))
            {
                option.receiveLeftClick(x, y);
                Game1.playSound("shwip");
            }
        }

        public override void draw(SpriteBatch b)
        {
            rootElement.Draw(b);
            drawMouse(b);
        }
    }
}
