using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewValley;

namespace ValleyReminders.ui
{
    internal abstract class Element
    {
        /*********
        ** Accessors
        *********/
        public object? UserData { get; set; }

        public Container? Parent { get; internal set; }
        public Vector2 LocalPosition { get; set; }
        public Vector2 Position
        {
            get
            {
                if (Parent != null)
                    return Parent.Position + LocalPosition;
                return LocalPosition;
            }
        }

        public abstract int Width { get; }
        public abstract int Height { get; }
        public Rectangle Bounds => new((int)Position.X, (int)Position.Y, Width, Height);

        public bool Hover { get; private set; }
        public virtual string HoveredSound => string.Empty;

        public virtual bool Clickable { get; set; } = true;
        public bool ClickGestured { get; private set; }
        public bool Clicked => Hover && ClickGestured;
        public virtual string ClickedSound => string.Empty;

        /// <summary>Whether to disable the element so it's invisible and can't be interacted with.</summary>
        public Func<bool>? ForceHide;

        /*********
        ** Public methods
        *********/
        /// <summary>Update the element for the current game tick.</summary>
        /// <param name="isOffScreen">Whether the element is currently off-screen.</param>
        public virtual void Update(bool isOffScreen = false)
        {
            bool hidden = IsHidden(isOffScreen);

            if (hidden)
            {
                Hover = false;
                ClickGestured = false;
                return;
            }

            int mouseX;
            int mouseY;
            if (Constants.TargetPlatform == GamePlatform.Android)
            {
                mouseX = Game1.getMouseX();
                mouseY = Game1.getMouseY();
            }
            else
            {
                mouseX = Game1.getOldMouseX();
                mouseY = Game1.getOldMouseY();
            }

            bool newHover = !hidden && !GetRoot().Obscured && Bounds.Contains(mouseX, mouseY);
            if (newHover && !Hover && HoveredSound != string.Empty)
                Game1.playSound(HoveredSound);
            Hover = newHover;

            if(Clickable)
            {
                ClickGestured = Game1.input.GetMouseState().LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released;
                ClickGestured = ClickGestured || (Game1.options.gamepadControls && (Game1.input.GetGamePadState().IsButtonDown(Buttons.A) && !Game1.oldPadState.IsButtonDown(Buttons.A)));
                if (Clicked)
                {
                    if (Parent != null)
                    {
                        ValleyReminders.Utilities.Monitor.Log($"Click gestured: {ClickGestured}");
                        ValleyReminders.Utilities.Monitor.Log($"Parent click consumed: {Parent.ClickConsumed}");
                        ValleyReminders.Utilities.Monitor.Log($"Type: {GetType().Name}");
                        ValleyReminders.Utilities.Monitor.Log("");
                        ClickGestured = !Parent.ClickConsumed; //If a click was already consumed, we have not actually been clicked
                        Parent.ClickConsumed = Parent.ClickConsumed || ClickGestured; //If the click has just been consumed, update the parent, otherwise it reassigns the same value
                    }

                    if ((Dropdown.SinceDropdownWasActive > 0 || Dropdown.ActiveDropdown != null))
                    {
                        ClickGestured = false;
                    }
                }

                if (Clicked && ClickedSound != string.Empty)
                    Game1.playSound(ClickedSound);
            }
        }

        public abstract void Draw(SpriteBatch b);

        public RootElement GetRoot()
        {
            return GetRootImpl();
        }

        internal virtual RootElement GetRootImpl()
        {
            if (Parent == null)
                throw new Exception("Element must have a parent.");
            return Parent.GetRoot();
        }

        /// <summary>Get whether the element is hidden based on <see cref="ForceHide"/> or its position relative to the screen.</summary>
        /// <param name="isOffScreen">Whether the element is currently off-screen.</param>
        public bool IsHidden(bool isOffScreen = false)
        {
            return isOffScreen || ForceHide?.Invoke() == true;
        }
    }
}
