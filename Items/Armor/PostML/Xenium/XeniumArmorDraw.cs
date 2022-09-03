using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Redemption.Items.Armor.PostML.Xenium
{
    class XeniumArmorDraw : PlayerDrawLayer
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }

        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.RedemptionPlayerBuff().xeniumBonus;

        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.BackAcc);
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            Texture2D texture = Request<Texture2D>("Redemption/Items/Armor/PostML/Xenium/XeniumGrenadeCannon").Value;
            Vector2 origin = new(texture.Width * 0.5f, texture.Height * 0.5f);
            Vector2 drawPos = drawPlayer.position + new Vector2(drawPlayer.width * 0.5f, drawPlayer.height * 0.5f);
            drawPos.X = drawPlayer.direction == 1 ? (int)drawPos.X + 3 : (int)drawPos.X - 3;
            DrawData drawData = new(texture, drawPos + (Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height] * drawPlayer.gravDir) - Main.screenPosition, new Rectangle?(), drawInfo.colorArmorBody, drawInfo.drawPlayer.bodyRotation, origin, 1, drawInfo.playerEffect, 0)
            {
                shader = drawInfo.cBody
            };
            drawInfo.DrawDataCache.Add(drawData);
        }
    }
}