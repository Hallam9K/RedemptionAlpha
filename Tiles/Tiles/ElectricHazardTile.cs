using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Microsoft.Xna.Framework.Graphics;

namespace Redemption.Tiles.Tiles
{
    public class ElectricHazardTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileSolid[Type] = false;
			Main.tileMergeDirt[Type] = false;
            Main.tileLighted[Type] = true;
            TileID.Sets.IsBeam[Type] = true;
            DustType = DustID.Electric;
            MinPick = 310;
            MineResist = 7f;
            HitSound = SoundID.Tink;
            AddMapEntry(new Color(200, 255, 255));
            AnimationFrameHeight = 90;
        }
        public override bool IsTileDangerous(int i, int j, Player player) => true;
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 4)
            {
                frameCounter = 0;
                frame++;
                if (frame > 2)
                    frame = 0;
            }
        }
        public override void NumDust(int i, int j, bool fail, ref int num)
		{
			num = fail ? 1 : 3;
		}
        public override void NearbyEffects(int i, int j, bool closer)
        {
            Player player = Main.LocalPlayer;
            float dist = Vector2.Distance(player.Center / 16f, new Vector2(i + 0.5f, j + 0.5f));
            if (dist <= 1)
            {
                player.AddBuff(BuffID.Electrified, 120);
            }
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;

            int height = tile.TileFrameY % AnimationFrameHeight >= 16 ? 18 : 16;
            int animate = Main.tileFrame[Type] * AnimationFrameHeight;
            Rectangle frame = new(tile.TileFrameX, tile.TileFrameY + animate, 16, height);
            Main.spriteBatch.Draw(texture, new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - (int)Main.screenPosition.Y) + zero, frame, Color.White, 0f, Vector2.Zero, 1f, 0, 0f);
        }
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.3f;
            g = 0.3f;
            b = 0.5f;
        }
        public override bool CanExplode(int i, int j) => false;
    }
}