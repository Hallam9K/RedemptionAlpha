using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Redemption.Globals.Player
{
    class XeniumArmorDraw : PlayerDrawLayer
    {
        public bool xeniumBonus;
        public override void SetStaticDefaults()
        { 
            base.SetStaticDefaults();
        }

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo)
        {
            return true;
        }

        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Torso);

        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Terraria.Player drawPlayer = drawInfo.drawPlayer;
            if (xeniumBonus)
            {
                Texture2D texture = ModContent.Request<Texture2D>("Redemption/Items/Armor/PostML/Xenium/XeniumGrenadeCannon").Value;
                Vector2 origin = new Vector2(texture.Width * 0.5f, texture.Height * 0.5f);
                SpriteEffects spriteEffects = drawPlayer.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
                Vector2 drawPos = drawPlayer.position + new Vector2(drawPlayer.width * 0.5f, drawPlayer.height * 0.5f);
                drawPos.X = drawPlayer.direction == 1 ? (int)drawPos.X + 3 : (int)drawPos.X - 3;
                drawPos.Y = (int)drawPos.Y + 2;
                DrawData data = new DrawData(texture, drawPos - Main.screenPosition, new Rectangle?(), Color.White, 0, origin, 1, spriteEffects, 0);
                drawInfo.DrawDataCache.Add(data);
            }          
        }      
    }
}