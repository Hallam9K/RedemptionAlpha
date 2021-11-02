using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;

namespace Redemption.Tiles.Tiles
{
    public class HalogenLampTile : ModTile
	{
		public override void SetStaticDefaults()
		{
            Main.tileSolid[Type] = false;
            Main.tileMergeDirt[Type] = false;
            Main.tileLighted[Type] = true;
            DustType = DustID.Electric;
            MinPick = 500;
            MineResist = 3f;
            SoundType = SoundID.Tink;
            AddMapEntry(new Color(193, 255, 219));
            AnimationFrameHeight = 90;
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
        public override void HitWire(int i, int j)
        {
            if (Main.tile[i, j].frameY >= 90)
                Main.tile[i, j].frameY -= 90;
            else
                Main.tile[i, j].frameY += 90;
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Main.tile[i, j];
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;

            int height = tile.frameY == 36 ? 18 : 16;
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>("Redemption/Tiles/Tiles/HalogenLampTile_Glow").Value, new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - (int)Main.screenPosition.Y) + zero, new Rectangle(tile.frameX, tile.frameY, 16, height), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Main.tile[i, j];
            if (tile.frameY < 90)
            {
                r = 0.0f;
                g = 0.3f;
                b = 0.3f;
            }
        }
        public override bool CanExplode(int i, int j)
        {
            return false;
        }
    }
}