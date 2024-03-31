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
            SpriteEffects effects = drawPlayer.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            Vector2 origin = new(texture.Width * 0.5f, texture.Height * 0.5f - 2);
            Vector2 drawPos = drawPlayer.position + new Vector2(drawPlayer.width * 0.5f, drawPlayer.height * 0.5f + drawPlayer.gfxOffY);
            drawPos.Y -= 2;
            float dir = 0;
            if (XeniumVisor.Activate(drawInfo.drawPlayer))
            {
                dir = MathHelper.Clamp((Main.MouseWorld - drawPlayer.Center).ToRotation(), -MathHelper.PiOver2, MathHelper.PiOver2);
                if (drawPlayer.direction == -1)
                {
                    dir = (Main.MouseWorld - drawPlayer.Center).ToRotation() - MathHelper.Pi;
                    if (dir < -MathHelper.PiOver2 && dir > -2 * MathHelper.PiOver2)
                        dir = MathHelper.PiOver2;
                    if (dir > -3 * MathHelper.PiOver2 && dir < -2 * MathHelper.PiOver2)
                        dir = -3 * MathHelper.PiOver2;
                }
            }
            drawPos.X = drawPlayer.direction == 1 ? (int)drawPos.X + 3 : (int)drawPos.X - 3;
            DrawData drawData = new(texture, drawPos + (Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height] * drawPlayer.gravDir) - Main.screenPosition, new Rectangle?(), drawInfo.colorArmorBody, drawInfo.drawPlayer.bodyRotation + dir, origin, 1, effects, 0)
            {
                shader = drawInfo.cBody
            };
            drawInfo.DrawDataCache.Add(drawData);
        }
    }
    public class XeniumArmorPlayer : ModPlayer
    {
        public override void PostUpdateMiscEffects()
        {
            if (Main.mouseItem.ModItem is XeniumGrenadeCannon && !XeniumVisor.Activate(Player))
            {
                Main.mouseItem.TurnToAir();
            }
        }
    }
}
