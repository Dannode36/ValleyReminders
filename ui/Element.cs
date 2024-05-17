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
        public object UserData { get; set; }

        public Container Parent { get; internal set; }
        public Vector2 LocalPosition { get; set; }
        public Vector2 Position
        {
            get
            {
                if (Parent != null)
                    return Parent.Position + this.LocalPosition;
                return LocalPosition;
            }
        }

        public abstract int Width { get; }
        public abstract int Height { get; }
        public Rectangle Bounds => new((int)this.Position.X, (int)Position.Y, this.Width, this.Height);

        public bool Hover { get; private set; }
        public virtual string HoveredSound => null;

        public bool ClickGestured { get; private set; }
        public bool Clicked => this.Hover && ClickGestured;
        public virtual string ClickedSound => null;

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
            if (newHover && !Hover && HoveredSound != null)
                Game1.playSound(HoveredSound);
            Hover = newHover;

            ClickGestured = (Game1.input.GetMouseState().LeftButton == ButtonState.Pressed && Game1.oldMouseState.LeftButton == ButtonState.Released);
            ClickGestured = ClickGestured || (Game1.options.gamepadControls && (Game1.input.GetGamePadState().IsButtonDown(Buttons.A) && !Game1.oldPadState.IsButtonDown(Buttons.A)));
            if (ClickGestured && (Dropdown.SinceDropdownWasActive > 0 || Dropdown.ActiveDropdown != null))
            {
                ClickGestured = false;
            }
            if (Clicked && ClickedSound != null)
                Game1.playSound(ClickedSound);
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
