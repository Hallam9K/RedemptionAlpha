using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Redemption.UI
{
    public class UIHoverTextImageButton : UIImageButton
    {
        public string Text;

        public UIHoverTextImageButton(Asset<Texture2D> texture, string text) : base(texture)
        {
            Text = text;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            if (IsMouseHovering)
            {
                Main.LocalPlayer.mouseInterface = true;
                Main.hoverItemName = Text;
            }
        }
    }
}