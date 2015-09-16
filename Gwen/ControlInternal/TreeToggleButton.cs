﻿using Gwen.Control;
using Newtonsoft.Json;

namespace Gwen.ControlInternal
{
    /// <summary>
    /// Tree node toggle button (the little plus sign).
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    [JsonConverter(typeof(Serialization.GwenConverter))]
    public class TreeToggleButton : Button
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeToggleButton"/> class.
        /// </summary>
        /// <param name="parent">Parent control.</param>
        public TreeToggleButton(ControlBase parent)
            : base(parent)
        {
            IsToggle = true;
            IsTabable = false;
        }

        /// <summary>
        /// Renders the focus overlay.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void renderFocus(Skin.SkinBase skin)
        {

        }

        /// <summary>
        /// Renders the control using specified skin.
        /// </summary>
        /// <param name="skin">Skin to use.</param>
        protected override void render(Skin.SkinBase skin)
        {
            skin.DrawTreeButton(this, ToggleState);
        }
    }
}
