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

        public void CreateInterface(List<Reminder> reminders)
        {
            this.reminders = reminders;
            initialize((Game1.uiViewport.Width / 2) - 500, (Game1.uiViewport.Height / 2) - 500, 1000, 800, true);
            UpdateInterface();
        }
        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            rootElement.OnHover(x, y);
        }

        public void UpdateInterface()
        {
            rootElement = new();

            Table table = new()
            {
                RowHeight = 100,
                Size = new(height, width),
                LocalPosition = new((Game1.uiViewport.Width / 2f) - 400, (Game1.uiViewport.Height / 2f) - 400)
            };

            {
                var checkBox = new Checkbox()
                {
                    Callback = (e) => { }
                };
                var label = new Label() { String = "Reminders Enabled", LocalPosition = new(checkBox.Width + 10, 0) };
                table.AddRow(new Element[] { checkBox, label });
            }

            {
                var button = new Button(Utilities.Helper.ModContent.Load<Texture2D>(Utilities.Button))
                {
                    Callback = (e) => { reminders.Clear(); UpdateInterface(); }
                };
                var label = new Label() { String = "Clear Reminders" };

                table.AddRow(new Element[] { button, label });
            }

            {
                //Reminders
                Table reminderTable = new(false)
                {
                    RowHeight = 20,
                    Size = new(600, 500),
                    LocalPosition = new(100, 0)
                };

                foreach (Reminder reminder in reminders)
                {
                    var textBox = new Label()
                    {
                        String = reminder.Message,
                        Font = Game1.smallFont
                        //LocalPosition = new(Position.X, Position.Y)
                    };
                    reminderTable.AddRow(new Element[] { textBox });
                }

                table.AddRow(new Element[] { reminderTable });
            }

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
            rootElement.HandleLeftClick(x, y);
        }

        public override void releaseLeftClick(int x, int y)
        {
            base.releaseLeftClick(x, y);
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
