using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Text.RegularExpressions;
using SMUI.Elements;
using SMUI.Elements.Pickers;
using ParameterData = System.Collections.Generic.KeyValuePair<string, ValleyReminders.ParameterType>;

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
        public const int Width = 832;
        public const int Height = 616;
        public static Vector2 CenterOffset => new((Game1.uiViewport.Width - Width) / 2, (Game1.uiViewport.Height - Height) / 2);

        private RootElement rootElement = new();
        private ReminderMenuState state = ReminderMenuState.LIST;

        //LIST
        private Table reminderListPage = new();
        private List<Reminder> reminders = new();
        bool reminderListDirty = false;

        //EDIT
        private StaticContainer reminderEditPage = new();
        private Reminder? selectedReminder = null;

        //CREATE
        private StaticContainer reminderCreatePage = new();
        private Reminder? newReminder = null;

        public void CreateInterface(List<Reminder> reminders)
        {
            this.reminders = reminders;

            initialize((int)CenterOffset.X, (int)CenterOffset.Y, Width, Height, true);
            upperRightCloseButton.setPosition(new(CenterOffset.X + Width + 20, CenterOffset.Y - 40));
            CreateStaticInterface();
            UpdateListPage();
        }

        public void OnOpen()
        {
            reminderListDirty = true;
            state = ReminderMenuState.LIST;
            selectedReminder = null;
            newReminder = null;
        }

        private void CreateStaticInterface()
        {
            rootElement = new();
            rootElement.AddChild(reminderListPage);
            rootElement.AddChild(reminderEditPage);
            rootElement.AddChild(reminderCreatePage);
        }

        public void UpdateListPage()
        {
            rootElement.RemoveChild(reminderListPage);

            reminderListPage = new()
            {
                RowHeight = 100,
                Size = new(Width, Height),
                LocalPosition = CenterOffset,
                RowPadding = 0
            };

            //Create button
            var createButton = new Button(Game1.mouseCursors, new(384, 396, 15, 15))
            {
                Size = new(Width + 40, reminderListPage.RowHeight + 4),
                Callback = (e) => { newReminder = new(); },
                LocalPosition = new(-20, 0)
            };

            var createButtonPlus = new Image()
            {
                Texture = Game1.mouseCursors,
                TextureArea = new(1, 412, 14, 14),
            };
            createButtonPlus.LocalPosition = new((createButton.Width - createButtonPlus.Width) / 2, 36);

            reminderListPage.AddRow(new Element[] { createButton, createButtonPlus });

            //Reminders
            foreach (Reminder reminder in reminders)
            {
                var button = new Button(Game1.mouseCursors, new(384, 396, 15, 15))
                {
                    Size = new(Width + 40, reminderListPage.RowHeight + 4),
                    Callback = (e) => { selectedReminder = reminder; },
                    LocalPosition = new(-20, 0)
                };

                var textBox = new Label()
                {
                    String = reminder.Name.Truncate(34),
                    Bold = true,
                    LocalPosition = new(0, 36)
                };

                var enabledCheck = new Checkbox()
                {
                    Checked = reminder.Enabled,
                    Callback = (e) => { reminder.Enabled = ((Checkbox)e).Checked; },
                };
                enabledCheck.LocalPosition = new(Width - 40, (reminderListPage.RowHeight - enabledCheck.Height) / 2 -24);

                var deleteButton = new Button(Game1.mouseCursors, new(323, 433, 9, 10))
                {
                    Callback = (e) => { reminders.Remove(reminder); reminderListDirty = true; },
                    BoxDraw = false
                };
                deleteButton.LocalPosition = new(Width - 40, (reminderListPage.RowHeight - deleteButton.Height) / 2 + 22);

                reminderListPage.AddRow(new Element[] { button, textBox, enabledCheck, deleteButton });
            }
            rootElement.AddChild(reminderListPage);
            reminderListDirty = false;
        }

        private void UpdateEditPage(Reminder selectedReminder)
        {
            rootElement.RemoveChild(reminderEditPage);

            reminderEditPage = new()
            {
                Size = new(Width, Height),
                LocalPosition = CenterOffset,
            };

            var msgTextBox = new Textbox()
            {
                String = selectedReminder.Message,
                Callback = (e) =>
                {
                    selectedReminder.Message = e.String;
                    reminderListDirty = true;
                }
            };
            reminderEditPage.AddChild(msgTextBox);

            var enabledCheckbox = new Checkbox()
            {
                Checked = selectedReminder.Enabled,
                Callback = (e) =>
                {
                    selectedReminder.Enabled = (e as Checkbox)!.Checked;
                    reminderListDirty = true;
                }
            };
            reminderEditPage.AddChild(enabledCheckbox);

            //Reminder conditions table
            var conditions = new Table()
            {
                RowHeight = 60,
                Size = new(width, height),
                RowSlip = 0
            };
            const int parameterMargin = 25; //for when condition input will be updated in reverse

            conditions.AddRow(new Element[] { new DateTimePicker() });

            //Display conditions
            foreach (var cond in selectedReminder.Conditions)
            {
                var conditionName = new Label()
                {
                    String = Regex.Replace(cond.MethodName, "([A-Z]+(?=$|[A-Z][a-z])|[A-Z]?[a-z]+)", " $1"),
                    Font = Game1.smallFont
                };

                List<Element> row = new();

                //Display condition parameters names and inputs
                int i = 0;
                int previousWidth = conditionName.Width;
                foreach (ParameterData condParameter in Conditions.GetParameterList(cond.MethodName))
                {
                    if (Conditions.GetParameterCount(cond.MethodName) > 1)
                    {
                        var condParameterLabel = new Label()
                        {
                            String = condParameter.Key,
                            Font = Game1.smallFont,
                            LocalPosition = new(previousWidth + 25, 0)
                        };
                        previousWidth += condParameterLabel.Width + 25;
                        row.Add(condParameterLabel);
                    }

                    int parameterIndex = i; //Temporary variable for lambda capture or ArgumentOutOfRangeException is bound to happen
                    switch (condParameter.Value)
                    {
                        case ParameterType.String:
                            Textbox condParameterTextBox = new()
                            {
                                String = cond.ParameterValues[i],
                                Callback = (e) =>
                                {
                                    cond.ParameterValues[parameterIndex] = e.String;
                                },
                                LocalPosition = new(previousWidth + 25, 0)
                            };
                            previousWidth += condParameterTextBox.Width + 25;
                            row.Add(condParameterTextBox);
                            break;
                        case ParameterType.Int:
                            Intbox condParameterIntBox = new()
                            {
                                String = cond.ParameterValues[i],
                                Callback = (e) =>
                                {
                                    cond.ParameterValues[parameterIndex] = e.String;
                                },
                                LocalPosition = new(previousWidth + 25, 0)
                            };
                            previousWidth += condParameterIntBox.Width + 25;
                            row.Add(condParameterIntBox);
                            break;
                        case ParameterType.Float:
                            Floatbox condParameterFloatBox = new()
                            {
                                String = cond.ParameterValues[i],
                                Callback = (e) =>
                                {
                                    cond.ParameterValues[parameterIndex] = e.String;
                                },
                                LocalPosition = new(previousWidth + 25, 0)
                            };
                            previousWidth += condParameterFloatBox.Width + 25;
                            row.Add(condParameterFloatBox);
                            break;
                        default:
                            throw new ArgumentException("Parameter type enum was not a known value. Please contanct mod author");
                    }
                    i++;
                }
                row.Add(conditionName);
                conditions.AddRow(row.ToArray());
            }
            reminderEditPage.AddChild(conditions);

            //Menu buttons
            var backButton = new Button(Game1.mouseCursors, new(352, 495, 12, 11))
            {
                Size = new(48, 44),
                LocalPosition = new Vector2(-96, -22),
                Callback = (e) => { this.selectedReminder = null; }
            };
            reminderEditPage.AddChild(backButton);

            rootElement.AddChild(reminderEditPage);
        }

        private void UpdateCreatePage()
        {
            rootElement.RemoveChild(reminderCreatePage);

            reminderCreatePage = new()
            {
                Size = new(Width, Height),
                LocalPosition = CenterOffset,
                OutlineColor = Color.White,
            };

            var backButton = new Button(Game1.mouseCursors, new(352, 495, 12, 11))
            {
                Size = new(48, 44),
                LocalPosition = new Vector2(-96, -22),
                Callback = (e) => { newReminder = null; }
            };
            reminderCreatePage.AddChild(backButton);

            rootElement.AddChild(reminderCreatePage);
        }

        public override void update(GameTime time)
        {
            base.update(time);

            //Self updating state. Feels a bit messy
            if(selectedReminder != null && state != ReminderMenuState.EDIT)
            {
                UpdateEditPage(selectedReminder);
                state = ReminderMenuState.EDIT;
            }
            else if (newReminder != null && state != ReminderMenuState.CREATE)
            {
                state = ReminderMenuState.CREATE;
                UpdateCreatePage();
                selectedReminder = null;
            }
            else if (selectedReminder == null && newReminder == null)
            {
                if (reminderListDirty) { UpdateListPage(); }
                state = ReminderMenuState.LIST;
            }

            reminderListPage.Enabled = false;
            reminderEditPage.Enabled = false;
            reminderCreatePage.Enabled = false;

            switch (state)
            {
                case ReminderMenuState.LIST:
                    reminderListPage.Enabled = true;
                    break;
                case ReminderMenuState.EDIT:
                    reminderEditPage.Enabled = true;
                    break;
                case ReminderMenuState.CREATE:
                    reminderEditPage.Enabled = true;
                    break;
                default:
                    break;
            }

            rootElement.Update();
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
                    Console.Write("a");
                    reminderCreatePage.Draw(b);
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
