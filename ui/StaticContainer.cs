using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace ValleyReminders.ui
{
    internal class StaticContainer : Container
    {
        public Vector2 Size { get; set; }

        public Color? OutlineColor { get; set; } = null;

        public override int Width => (int)this.Size.X;

        public override int Height => (int)this.Size.Y;

        public override void Draw(SpriteBatch b)
        {
            if (IsHidden())
                return;
            if (OutlineColor.HasValue)
            {
                IClickableMenu.drawTextureBox(b, (int)Position.X - 12, (int)Position.Y - 12, Width + 24, Height + 24, OutlineColor.Value);
            }
            base.Draw(b);
        }
    }
}
