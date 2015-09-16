﻿using System;
using Gwen.ControlInternal;
using Newtonsoft.Json;

namespace Gwen.Control
{
    /// <summary>
    /// CollapsibleCategory control. Used in CollapsibleList.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    [JsonConverter(typeof(Serialization.GwenConverter))]
    public class CollapsibleCategory : ControlBase
    {
        private readonly Button headerButton;
        private readonly CollapsibleList list;

        /// <summary>
        /// Header text.
        /// </summary>
        public string Text { get { return headerButton.Text; } set { headerButton.Text = value; } }

        /// <summary>
        /// Determines whether the category is collapsed (closed).
        /// </summary>
        public bool IsCollapsed { get { return headerButton.ToggleState; } set { headerButton.ToggleState = value; } }

        /// <summary>
        /// Invoked when an entry has been selected.
        /// </summary>
		public event GwenEventHandler<ItemSelectedEventArgs> Selected;

        /// <summary>
        /// Invoked when the category collapsed state has been changed (header button has been pressed).
        /// </summary>
		public event GwenEventHandler<EventArgs> Collapsed;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollapsibleCategory"/> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        [Newtonsoft.Json.JsonConstructor]
        public CollapsibleCategory(CollapsibleList parent) : base(parent)
        {
            headerButton = new CategoryHeaderButton(this);
            headerButton.Text = "Category Title"; // [omeg] todo: i18n
            headerButton.Dock = Pos.Top;
            headerButton.Height = 20;
            headerButton.Toggled += onHeaderToggle;

            list = parent;

            Padding = new Padding(1, 0, 1, 5);
            SetSize(512, 512);
        }

        /// <summary>
        /// Gets the selected entry.
        /// </summary>
        public Button GetSelectedButton()
        {
            foreach (ControlBase child in Children)
            {
                CategoryButton button = child as CategoryButton;
                if (button == null)
                    continue;

                if (button.ToggleState)
                    return button;
            }

            return null;
        }

        /// <summary>
        /// Handler for header button toggle event.
        /// </summary>
        /// <param name="control">Source control.</param>
		protected virtual void onHeaderToggle(ControlBase control, EventArgs args)
        {
            if (Collapsed != null)
				Collapsed.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handler for Selected event.
        /// </summary>
        /// <param name="control">Event source.</param>
		protected virtual void onSelected(ControlBase control, EventArgs args)
        {
            CategoryButton child = control as CategoryButton;
            if (child == null) return;

            if (list != null)
            {
                list.UnselectAll();
            }
            else
            {
                UnselectAll();
            }

            child.ToggleState = true;

            if (Selected != null)
                Selected.Invoke(this, new ItemSelectedEventArgs(control));
        }

        /// <summary>
        /// Adds a new entry.
        /// </summary>
        /// <param name="name">Entry name (displayed).</param>
        /// <returns>Newly created control.</returns>
        public Button Add(string name)
        {
            CategoryButton button = new CategoryButton(this);
            button.Text = name;
            button.Dock = Pos.Top;
            button.SizeToContents();
            button.SetSize(button.Width + 4, button.Height + 4);
            button.Padding = new Padding(5, 2, 2, 2);
            button.Clicked += onSelected;

            return button;
        }

        /// <summary>
        /// Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void render(Skin.SkinBase skin)
        {
            skin.DrawCategoryInner(this, headerButton.ToggleState);
            base.render(skin);
        }

        /// <summary>
        /// Unselects all entries.
        /// </summary>
        public void UnselectAll()
        {
            foreach (ControlBase child in Children)
            {
                CategoryButton button = child as CategoryButton;
                if (button == null)
                    continue;

                button.ToggleState = false;
            }
        }

        /// <summary>
        /// Function invoked after layout.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void postLayout(Skin.SkinBase skin)
        {
            if (IsCollapsed)
            {
                Height = headerButton.Height;
            }
            else
            {
                SizeToChildren(false, true);
            }

            // alternate row coloring
            bool b = true;
            foreach (ControlBase child in Children)
            {
                CategoryButton button = child as CategoryButton;
                if (button == null)
                    continue;

                button.alt = b;
                button.UpdateColors();
                b = !b;
            }
        }
    }
}
