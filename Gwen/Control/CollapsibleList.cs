﻿using System;
using Newtonsoft.Json;

namespace Gwen.Control
{
    /// <summary>
    /// CollapsibleList control. Groups CollapsibleCategory controls.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    [JsonConverter(typeof(Serialization.GwenConverter))]
    public class CollapsibleList : ScrollControl
    {
        /// <summary>
        /// Invoked when an entry has been selected.
        /// </summary>
        public event GwenEventHandler<ItemSelectedEventArgs> ItemSelected;

        /// <summary>
        /// Invoked when a category collapsed state has been changed (header button has been pressed).
        /// </summary>
        public event GwenEventHandler<EventArgs> CategoryCollapsed;

        /// <summary>
        /// Initializes a new instance of the <see cref="CollapsibleList"/> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public CollapsibleList(ControlBase parent) : base(parent)
        {
			MouseInputEnabled = true;
            EnableScroll(false, true);
            AutoHideBars = true;
        }

        // todo: iterator, make this as function? check if works

        /// <summary>
        /// Selected entry.
        /// </summary>
        public Button GetSelectedButton()
        {
            foreach (ControlBase child in Children)
            {
                CollapsibleCategory cat = child as CollapsibleCategory;
                if (cat == null)
                    continue;

                Button button = cat.GetSelectedButton();

                if (button != null)
                    return button;
            }

            return null;
        }

        /// <summary>
        /// Adds a category to the list.
        /// </summary>
        /// <param name="category">Category control to add.</param>
        protected virtual void add(CollapsibleCategory category)
        {
            category.Parent = this;
            category.Dock = Pos.Top;
            category.Margin = new Margin(1, 0, 1, 1);
            category.Selected += onCategorySelected;
            category.Collapsed += onCategoryCollapsed;
            // this relies on fact that category.m_List is set to its parent
        }

        /// <summary>
        /// Adds a new category to the list.
        /// </summary>
        /// <param name="categoryName">Name of the category.</param>
        /// <returns>Newly created control.</returns>
        public virtual CollapsibleCategory Add(string categoryName)
        {
            CollapsibleCategory cat = new CollapsibleCategory(this);
            cat.Text = categoryName;
            add(cat);
            return cat;
        }

        /// <summary>
        /// Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void render(Skin.SkinBase skin)
        {
            skin.DrawCategoryHolder(this);
            base.render(skin);
        }

        /// <summary>
        /// Unselects all entries.
        /// </summary>
        public virtual void UnselectAll()
        {
            foreach (ControlBase child in Children)
            {
                CollapsibleCategory cat = child as CollapsibleCategory;
                if (cat == null)
                    continue;

                cat.UnselectAll();
            }
        }

        /// <summary>
        /// Handler for ItemSelected event.
        /// </summary>
        /// <param name="control">Event source: <see cref="CollapsibleList"/>.</param>
		protected virtual void onCategorySelected(ControlBase control, EventArgs args)
        {
            CollapsibleCategory cat = control as CollapsibleCategory;
            if (cat == null) return;

            if (ItemSelected != null)
                ItemSelected.Invoke(this, new ItemSelectedEventArgs(cat));
        }

        /// <summary>
        /// Handler for category collapsed event.
        /// </summary>
        /// <param name="control">Event source: <see cref="CollapsibleCategory"/>.</param>
        protected virtual void onCategoryCollapsed(ControlBase control, EventArgs args)
        {
            CollapsibleCategory cat = control as CollapsibleCategory;
            if (cat == null) return;

            if (CategoryCollapsed != null)
                CategoryCollapsed.Invoke(control, EventArgs.Empty);
        }
    }
}
