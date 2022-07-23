using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Redemption.Items.Armor.Vanity
{
    public class SwordHeadbandDraw_Front : PlayerDrawLayer
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.head == EquipLoader.GetEquipSlot(Mod, nameof(SwordHeadband), EquipType.Head);
        public override Position GetDefaultPosition() => new AfterParent(PlayerDrawLayers.Head);
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            Color color = drawPlayer.GetImmuneAlphaPure(drawInfo.colorArmorHead, drawInfo.shadow);

            Texture2D texture = Request<Texture2D>("Redemption/Items/Armor/Vanity/SwordHeadband_Front").Value;
            Vector2 Position = drawInfo.Position;
            Vector2 origin = new(texture.Width * 0.5f, texture.Height * 0.5f);
            Vector2 drawPos = new Vector2((int)(Position.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2), (int)(Position.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2);
            drawPos.Y -= 12 * drawPlayer.gravDir;
            DrawData drawData = new(texture, drawPos + (Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height] * drawPlayer.gravDir) - Main.screenPosition, new Rectangle?(), color, drawInfo.drawPlayer.headRotation, origin, 1, drawInfo.playerEffect, 0)
            {
                shader = drawInfo.cHead
            };
            drawInfo.DrawDataCache.Add(drawData);
        }
    }
    public class SwordHeadbandDraw_Back : PlayerDrawLayer
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
        }
        public override bool GetDefaultVisibility(PlayerDrawSet drawInfo) => drawInfo.drawPlayer.head == EquipLoader.GetEquipSlot(Mod, nameof(SwordHeadband), EquipType.Head);
        public override Position GetDefaultPosition() => new BeforeParent(PlayerDrawLayers.Head);
        protected override void Draw(ref PlayerDrawSet drawInfo)
        {
            Player drawPlayer = drawInfo.drawPlayer;
            Color color = drawPlayer.GetImmuneAlphaPure(drawInfo.colorArmorHead, drawInfo.shadow);

            Texture2D texture = Request<Texture2D>("Redemption/Items/Armor/Vanity/SwordHeadband_Back").Value;
            Vector2 Position = drawInfo.Position;
            Vector2 origin = new(texture.Width * 0.5f, texture.Height * 0.5f);
            Vector2 drawPos = new Vector2((int)(Position.X - drawPlayer.bodyFrame.Width / 2 + drawPlayer.width / 2), (int)(Position.Y + drawPlayer.height - drawPlayer.bodyFrame.Height + 4f)) + drawPlayer.bodyPosition + new Vector2(drawPlayer.bodyFrame.Width / 2, drawPlayer.bodyFrame.Height / 2);
            drawPos.Y -= 12 * drawPlayer.gravDir;
            DrawData drawData = new(texture, drawPos + (Main.OffsetsPlayerHeadgear[drawInfo.drawPlayer.bodyFrame.Y / drawInfo.drawPlayer.bodyFrame.Height] * drawPlayer.gravDir) - Main.screenPosition, new Rectangle?(), color, drawInfo.drawPlayer.headRotation, origin, 1, drawInfo.playerEffect, 0)
            {
                shader = drawInfo.cHead
            };
            drawInfo.DrawDataCache.Add(drawData);
        }
    }
}