using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System.Text.RegularExpressions;
using SMUI.Elements;
using SMUI.Elements.Pickers;
using ParameterData = System.Collections.Generic.KeyValuePair<string, ValleyReminders.ParameterType>;
using SMUI;
using Microsoft.Xna.Framework.Input;
using SMUI.SDV;

namespace ValleyReminders
{
    enum ReminderMenuState
    {
        LIST,
        EDIT,
        CREATE
    }

    class ReminderMenu : SmuiMenu
    {
        public const int Width = 832;
        public const int Height = 616;
        public static Vector2 CenterOffset => new((Game1.uiViewport.Width - Width) / 2, (Game1.uiViewport.Height - Height) / 2);

        private ReminderMenuState state = ReminderMenuState.LIST;

        //LIST
        private Table reminderListPage = new();
        private List<Reminder> reminders = new();
        bool reminderListDirty = false;

        //EDIT
        private Panel reminderEditPage = new();
        private Reminder? selectedReminder = null;

        //CREATE
        private Panel reminderCreatePage = new();
        private Reminder? newReminder = null;

        public void CreateInterface(List<Reminder> reminders)
        {
            this.reminders = reminders;
            Root = new();

            initialize((int)CenterOffset.X, (int)CenterOffset.Y, Width, Height, true);
            upperRightCloseButton.setPosition(new(CenterOffset.X + Width + 20, CenterOffset.Y - 40));

            reminderListPage = new()
            {
                RowHeight = 100,
                Size = new(Width, Height),
                Position = CenterOffset,
                RowPadding = 0
            };

            reminderEditPage = new()
            {
                Size = new(Width, Height),
                Position = CenterOffset,
            };

            reminderCreatePage = new()
            {
                Size = new(Width, Height),
                Position = CenterOffset,
                OutlineColor = Color.White,
            };

            AddChild(reminderListPage);
            AddChild(reminderEditPage);
            AddChild(reminderCreatePage);

            /*UpdateListPage();
            UpdateCreatePage();*/
        }

        public void OnOpen()
        {
            reminderListDirty = true;
            state = ReminderMenuState.LIST;
            selectedReminder = null;
            newReminder = null;
            CreateInterface(reminders);
        }

        public void UpdateListPage()
        {
            reminderListPage.ClearChildren();

            //Create button
            var createButton = new Button(Game1.mouseCursors, new(384, 396, 15, 15))
            {
                DrawSize = new(Width + 40, reminderListPage.RowHeight + 4),
                OnClick = (e) => { newReminder = new(); },
                Position = new(-20, 0)
            };

            var createButtonPlus = new Image(Game1.mouseCursors, new(1, 412, 14, 14));
            createButtonPlus.Position = new((createButton.Width - createButtonPlus.Width) / 2, 36);

            reminderListPage.AddRow(new Element[] { createButton, createButtonPlus });

            //Reminders
            foreach (Reminder reminder in reminders)
            {
                var button = new Button(Game1.mouseCursors, new(384, 396, 15, 15))
                {
                    DrawSize = new(Width + 40, reminderListPage.RowHeight + 4),
                    OnClick = (e) => { selectedReminder = reminder; },
                    Position = new(-20, 0)
                };

                var TextInput = new Text()
                {
                    String = reminder.Name.Truncate(34),
                    Bold = true,
                    Position = new(0, 36)
                };

                var enabledCheck = new Checkbox()
                {
                    Checked = reminder.Enabled,
                    OnClick = (e) => { reminder.Enabled = ((Checkbox)e).Checked; },
                };
                enabledCheck.Position = new(Width - 40, (reminderListPage.RowHeight - enabledCheck.Height) / 2 -24);

                var deleteButton = new Button(Game1.mouseCursors, new(323, 433, 9, 10))
                {
                    OnClick = (e) => { reminders.Remove(reminder); reminderListDirty = true; },
                    DrawToFit = false
                };
                deleteButton.Position = new(Width - 40, (reminderListPage.RowHeight - deleteButton.Height) / 2 + 22);

                reminderListPage.AddRow(new Element[] { button, TextInput, enabledCheck, deleteButton });
            }
            reminderListDirty = false;
        }

        private void UpdateEditPage(Reminder selectedReminder)
        {
            reminderEditPage.ClearChildren();

            var msgTextInput = new TextInput()
            {
                String = selectedReminder.Message,
                OnValueChange = (e) =>
                {
                    selectedReminder.Message = e.String;
                    reminderListDirty = true;
                }
            };
            reminderEditPage.AddChild(msgTextInput);

            var enabledCheckbox = new Checkbox()
            {
                Checked = selectedReminder.Enabled,
                OnClick = (e) =>
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
                RowOffset = 0
            };
            const int parameterMargin = 25; //for when condition input will be updated in reverse

            conditions.AddRow(new Element[] { new DateTimePickerPopup() });

            //Display conditions
            foreach (var cond in selectedReminder.Conditions)
            {
                var conditionName = new Text()
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
                        var condParameterLabel = new Text()
                        {
                            String = condParameter.Key,
                            Font = Game1.smallFont,
                            Position = new(previousWidth + 25, 0)
                        };
                        previousWidth += condParameterLabel.Width + 25;
                        row.Add(condParameterLabel);
                    }

                    int parameterIndex = i; //Temporary variable for lambda capture or ArgumentOutOfRangeException is bound to happen
                    switch (condParameter.Value)
                    {
                        case ParameterType.String:
                            TextInput condParameterTextInput = new()
                            {
                                String = cond.ParameterValues[i],
                                OnValueChange = (e) =>
                                {
                                    cond.ParameterValues[parameterIndex] = e.String;
                                },
                                Position = new(previousWidth + 25, 0)
                            };
                            previousWidth += condParameterTextInput.Width + 25;
                            row.Add(condParameterTextInput);
                            break;
                        case ParameterType.Int:
                            IntInput condParameterIntBox = new()
                            {
                                String = cond.ParameterValues[i],
                                OnValueChange = (e) =>
                                {
                                    cond.ParameterValues[parameterIndex] = e.String;
                                },
                                Position = new(previousWidth + 25, 0)
                            };
                            previousWidth += condParameterIntBox.Width + 25;
                            row.Add(condParameterIntBox);
                            break;
                        case ParameterType.Float:
                            FloatInput condParameterFloatBox = new()
                            {
                                String = cond.ParameterValues[i],
                                OnValueChange = (e) =>
                                {
                                    cond.ParameterValues[parameterIndex] = e.String;
                                },
                                Position = new(previousWidth + 25, 0)
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
                DrawSize = new(48, 44),
                Position = new Vector2(-96, -22),
                OnClick = (e) => { this.selectedReminder = null; }
            };
            reminderEditPage.AddChild(backButton);
        }

        private void UpdateCreatePage()
        {
            reminderCreatePage.ClearChildren();

            var backButton = new Button(Game1.mouseCursors, new(352, 495, 12, 11))
            {
                DrawSize = new(48, 44),
                Position = new Vector2(-96, -22),
                OnClick = (e) => { newReminder = null; }
            };
            reminderCreatePage.AddChild(backButton);

            var createButton = new Button(Game1.mouseCursors, new(341, 410, 23, 9))
            {
                DrawSize = new(46, 18),
                Position = new Vector2(20, 100),
                OnClick = (e) => { reminders.Add(newReminder!); reminderListDirty = true; newReminder = null; }
            };
            reminderCreatePage.AddChild(createButton);
        }

        public override void update(GameTime time)
        {
            base.update(time);

            //Self updating state. Feels a bit messy
            if (selectedReminder != null && state != ReminderMenuState.EDIT)
            {
                state = ReminderMenuState.EDIT;
                UpdateEditPage(selectedReminder);

                reminderListPage.Enabled = false;
                reminderEditPage.Enabled = true;
                reminderCreatePage.Enabled = false;
            }
            else if (newReminder != null && state != ReminderMenuState.CREATE)
            {
                state = ReminderMenuState.CREATE;
                UpdateCreatePage();
                selectedReminder = null;

                reminderListPage.Enabled = false;
                reminderEditPage.Enabled = false;
                reminderCreatePage.Enabled = true;
            }
            else if (selectedReminder == null && newReminder == null)
            {
                state = ReminderMenuState.LIST;
                if (reminderListDirty) { UpdateListPage(); }

                reminderListPage.Enabled = true;
                reminderEditPage.Enabled = false;
                reminderCreatePage.Enabled = false;
            }
        }

        public override void receiveScrollWheelAction(int direction)
        {
            base.receiveScrollWheelAction(direction);
            if(reminderListPage.Enabled)
            {
               // reminderListPage.Scrollbar.ScrollBy(direction);
            }
        }
    }
}
