﻿using System;
using System.Drawing;
using Gwen.ControlInternal;
using Gwen.DragDrop;
using Newtonsoft.Json;

namespace Gwen.Control
{
    /// <summary>
    /// Tab strip - groups TabButtons and allows reordering.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    [JsonConverter(typeof(Serialization.GwenConverter))]
    public class TabStrip : ControlBase
    {
        private ControlBase tabDragControl;
        private bool allowReorder;

        /// <summary>
        /// Determines whether it is possible to reorder tabs by mouse dragging.
        /// </summary>
        public bool AllowReorder { get { return allowReorder; } set { allowReorder = value; } }

        /// <summary>
        /// Determines whether the control should be clipped to its bounds while rendering.
        /// </summary>
        protected override bool shouldClip
        {
            get { return false; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TabStrip"/> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public TabStrip(ControlBase parent)
            : base(parent)
        {
            allowReorder = false;
        }

        /// <summary>
        /// Strip position (top/left/right/bottom).
        /// </summary>
        public Pos StripPosition
        {
            get { return Dock; }
            set
            {
                Dock = value;
                if (Dock == Pos.Top)
                    Padding = new Padding(5, 0, 0, 0);
                if (Dock == Pos.Left)
                    Padding = new Padding(0, 5, 0, 0);
                if (Dock == Pos.Bottom)
                    Padding = new Padding(5, 0, 0, 0);
                if (Dock == Pos.Right)
                    Padding = new Padding(0, 5, 0, 0);
            }
        }

        public override bool DragAndDrop_HandleDrop(Package p, int x, int y)
        {
            Point LocalPos = CanvasPosToLocal(new Point(x, y));

            TabButton button = DragAndDrop.SourceControl as TabButton;
            TabControl tabControl = Parent as TabControl;
            if (tabControl != null && button != null)
            {
                if (button.TabControl != tabControl)
                {
                    // We've moved tab controls!
                    tabControl.AddPage(button);
                }
            }

            ControlBase droppedOn = GetControlAt(LocalPos.X, LocalPos.Y);
            if (droppedOn != null)
            {
                Point dropPos = droppedOn.CanvasPosToLocal(new Point(x, y));
                DragAndDrop.SourceControl.BringNextToControl(droppedOn, dropPos.X > droppedOn.Width/2);
            }
            else
            {
                DragAndDrop.SourceControl.BringToFront();
            }
            return true;
        }

        public override bool DragAndDrop_CanAcceptPackage(Package p)
        {
            if (!allowReorder)
                return false;

            if (p.Name == "TabButtonMove")
                return true;

            return false;
        }

        /// <summary>
        /// Lays out the control's interior according to alignment, padding, dock etc.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void layout(Skin.SkinBase skin)
        {
            Point largestTab = new Point(5, 5);

            int num = 0;
            foreach (var child in Children)
            {
                TabButton button = child as TabButton;
                if (null == button) continue;

                button.SizeToContents();

                Margin m = new Margin();
                int notFirst = num > 0 ? -1 : 0;

                if (Dock == Pos.Top)
                {
                    m.Left = notFirst;
                    button.Dock = Pos.Left;
                }

                if (Dock == Pos.Left)
                {
                    m.Top = notFirst;
                    button.Dock = Pos.Top;
                }

                if (Dock == Pos.Right)
                {
                    m.Top = notFirst;
                    button.Dock = Pos.Top;
                }

                if (Dock == Pos.Bottom)
                {
                    m.Left = notFirst;
                    button.Dock = Pos.Left;
                }

                largestTab.X = Math.Max(largestTab.X, button.Width);
                largestTab.Y = Math.Max(largestTab.Y, button.Height);

                button.Margin = m;
                num++;
            }

            if (Dock == Pos.Top || Dock == Pos.Bottom)
                SetSize(Width, largestTab.Y);

            if (Dock == Pos.Left || Dock == Pos.Right)
                SetSize(largestTab.X, Height);

            base.layout(skin);
        }

        public override void DragAndDrop_HoverEnter(Package p, int x, int y)
        {
            if (tabDragControl != null)
            {
                throw new InvalidOperationException("ERROR! TabStrip::DragAndDrop_HoverEnter");
            }

            tabDragControl = new Highlight(this);
            tabDragControl.MouseInputEnabled = false;
            tabDragControl.SetSize(3, Height);
        }

        public override void DragAndDrop_HoverLeave(Package p)
        {
            if (tabDragControl != null)
            {
                RemoveChild(tabDragControl, false); // [omeg] need to do that explicitely
                tabDragControl.Dispose();
            }
            tabDragControl = null;
        }

        public override void DragAndDrop_Hover(Package p, int x, int y)
        {
            Point localPos = CanvasPosToLocal(new Point(x, y));

            ControlBase droppedOn = GetControlAt(localPos.X, localPos.Y);
            if (droppedOn != null && droppedOn != this)
            {
                Point dropPos = droppedOn.CanvasPosToLocal(new Point(x, y));
                tabDragControl.SetBounds(new Rectangle(0, 0, 3, Height));
                tabDragControl.BringToFront();
                tabDragControl.SetPosition(droppedOn.X - 1, 0);

                if (dropPos.X > droppedOn.Width/2)
                {
                    tabDragControl.MoveBy(droppedOn.Width - 1, 0);
                }
                tabDragControl.Dock = Pos.None;
            }
            else
            {
                tabDragControl.Dock = Pos.Left;
                tabDragControl.BringToFront();
            }
        }
    }
}
