using System;
using System.Threading;
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

        public bool Hovered { get; private set; }
        public virtual string HoveredSound => string.Empty; //TODO: what is this?

        public bool ClickGestured { get; private set; }
        public bool Clicked => this.Hovered && this.ClickGestured;
        public virtual string ClickedSound => null;

        /// <summary>Whether to disable the element so it's invisible and can't be interacted with.</summary>
        public Func<bool> ForceHide;

        /*********
        ** Public methods
        *********/
        /// <summary>Update the element for the current game tick.</summary>
        /// <param name="isOffScreen">Whether the element is currently off-screen.</param>
        public virtual void HandleLeftClick(int x, int y)
        {
            if (IsHidden())
            {
                Hovered = false;
                ClickGestured = false;
                return;
            }

            Hovered = !GetRoot().Obscured && Bounds.Contains(x, y);
            ClickGestured = Bounds.Contains(x, y);

            if (ClickGestured && (Dropdown.SinceDropdownWasActive > 0 || Dropdown.ActiveDropdown != null))
            {
                ClickGestured = false;
            }
            if (Clicked && ClickedSound != null)
                Game1.playSound(ClickedSound);
        }

        public virtual void OnHover(int x, int y)
        {
            bool beginHover = !GetRoot().Obscured && Bounds.Contains(x, y);
            if (beginHover && !Hovered && HoveredSound != null)
                Game1.playSound(HoveredSound);
            Hovered = beginHover;
        }

        public abstract void Draw(SpriteBatch b);

        public RootElement GetRoot()
        {
            return GetRootInternal();
        }

        internal virtual RootElement GetRootInternal()
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
