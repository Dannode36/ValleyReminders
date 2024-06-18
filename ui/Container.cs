using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using static System.Net.Mime.MediaTypeNames;

namespace ValleyReminders.ui
{
    internal abstract class Container : Element
    {
        /*********
        ** Fields
        *********/
        private readonly IList<Element> ChildrenImpl = new List<Element>();

        /// <summary>Whether to update the <see cref="Children"/> when <see cref="Update"/> is called.</summary>
        protected bool UpdateChildren { get; set; } = true;


        /*********
        ** Accessors
        *********/
        private Element? renderLast = null;
        public Element? RenderLast
        {
            get => renderLast;
            set {
                renderLast = value;
                if (Parent is not null) {
                    if (value is null) {
                        if (Parent.RenderLast == this) {
                            Parent.RenderLast = null;
                        }
                    } else {
                        Parent.RenderLast = this;
                    }
                }
            }
        }

        public Element[] Children => ChildrenImpl.ToArray();

        public bool Selected { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        /*********
        ** Public methods
        *********/
        public void AddChild(Element element)
        {
            element.Parent?.RemoveChild(element);
            ChildrenImpl.Add(element);
            element.Parent = this;
        }

        public void RemoveChild(Element element)
        {
            if (element.Parent != this)
                throw new ArgumentException("Element must be a child of this container.");
            ChildrenImpl.Remove(element);
            element.Parent = null;
        }

        /// <inheritdoc />
        public override void Update(bool isOffScreen = false)
        {
            base.Update(isOffScreen);
            if (UpdateChildren)
            {
                foreach (var element in ChildrenImpl)
                    element.Update(isOffScreen);
            }
        }

        /// <inheritdoc />
        public override void Draw(SpriteBatch b)
        {
            if (IsHidden())
                return;

            foreach (var child in   ChildrenImpl)
            {
                if (child == RenderLast)
                    continue;
                child.Draw(b);
            }
            RenderLast?.Draw(b);
        }
    }
}
