using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Accessories.PostML;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Shade
{
    public class CelestineDreamsongTile : ModTile
	{
        public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.addTile(Type);
            DustType = DustID.AncientLight;
            HitSound = SoundID.Tink;
			ModTranslation name = CreateMapEntryName();
            name.SetDefault("Celestine Dreamsong");
            AddMapEntry(new Color(223, 230, 238));
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 1f;
            g = 1f;
            b = 1f;
        }
        public override bool RightClick(int i, int j)
        {
            Main.player[Main.myPlayer].PickTile(i, j, 100);
            return true;
        }
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<CelestineDreamsong>();
        }
        public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
        {
            int left = i - Main.tile[i, j].TileFrameX / 18 % 2;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 2;
            if (Main.tile[left, top].TileFrameX != 0)
                return true;

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/WhiteFlare").Value;
            Texture2D glow = ModContent.Request<Texture2D>("Redemption/Textures/WhiteGlow").Value;
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;
            Vector2 origin = new(flare.Width / 2f, flare.Height / 2f);
            Vector2 origin2 = new(glow.Width / 2f, glow.Height / 2f);
            if (Main.tile[i, j].TileFrameX == 0 && Main.tile[i, j].TileFrameY == 0)
            {
                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);

                spriteBatch.Draw(glow, new Vector2(((i + 1f) * 16) - (int)Main.screenPosition.X, ((j + 1f) * 16) - (int)Main.screenPosition.Y) + zero, null, RedeColor.FadeColour1, 0, origin2, 2f, SpriteEffects.None, 1f);
                spriteBatch.Draw(flare, new Vector2(((i + 1f) * 16) - (int)Main.screenPosition.X, ((j + 1f) * 16) - (int)Main.screenPosition.Y) + zero, null, RedeColor.COLOR_GLOWPULSE, Main.GlobalTimeWrappedHourly, origin, 1.3f, SpriteEffects.None, 1f);
                spriteBatch.Draw(flare, new Vector2(((i + 1f) * 16) - (int)Main.screenPosition.X, ((j + 1f) * 16) - (int)Main.screenPosition.Y) + zero, null, RedeColor.COLOR_GLOWPULSE, -Main.GlobalTimeWrappedHourly, origin, 1.3f, SpriteEffects.None, 1f);

                spriteBatch.End();
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null);
            }
            return true;
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 32, 32, ModContent.ItemType<CelestineDreamsong>());
        }
        public override bool CanExplode(int i, int j) => false;
    }
}