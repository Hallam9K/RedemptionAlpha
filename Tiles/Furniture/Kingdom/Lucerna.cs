using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Kingdom
{
    public class Lucerna : ModTile
	{
        public override void SetStaticDefaults()
		{
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 1;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);
            DustType = DustID.GoldCoin;
            MinPick = 1000;
            MineResist = 30f;
            LocalizedText name = CreateMapEntryName();
            //name.SetDefault("Lucerna");
            AddMapEntry(new Color(206, 150, 15), name);
		}
        public override bool CanExplode(int i, int j) => false;
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int width = 20;
            int offsetY = 0;

            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;

            Vector2 baseDrawPosition = new Vector2(i * 16 - (int)Main.screenPosition.X - (width - 16f) / 2f, j * 16 - (int)Main.screenPosition.Y + offsetY) + zero;
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/HolyGlow").Value;
            float glowMod = (float)Math.Sin(Main.GameUpdateCount / 5);
            float glowSize = 0.3f + 0.01f * glowMod;

            Color glowColor = Color.White * 0.3f;
            glowColor.A = 0;
            spriteBatch.Draw(glow, baseDrawPosition + new Vector2(10, 6), null, glowColor, 0f, new Vector2(glow.Width / 2, glow.Height / 2), glowSize, SpriteEffects.None, 0f);


            Texture2D bloom = Redemption.WhiteOrb.Value;
            float bloomMod = (float)Math.Sin(Main.GameUpdateCount / 10);
            float bloomSize = 1f + 0.2f * bloomMod;
            Color bloomColor = new Color(206, 150, 15) * 0.5f;
            bloomColor.A = 0;
            spriteBatch.Draw(bloom, baseDrawPosition + new Vector2(10, 6), null, bloomColor, 0f, new Vector2(bloom.Width / 2, bloom.Height / 2), bloomSize, SpriteEffects.None, 0f);
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            float glowMod = (float)Math.Sin(Main.GameUpdateCount / 5);
            float glowSize = 2f + 0.07f * glowMod;
            r = 0.206f * 3f * glowSize;
            g = 0.150f * 3f * glowSize;
            b = 0.015f * 3f * glowSize;
        }
    }
    public class LucernaItem : PlaceholderTile
    {
        public override string Texture => "Redemption/Placeholder";
        public override void SetSafeStaticDefaults()
        {
            //DisplayName.SetDefault("Lucerna");
            //Tooltip.SetDefault("[c/ff0000:Unbreakable (1000% Pickaxe Power)]");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createTile = ModContent.TileType<Lucerna>();
        }
    }
}