using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Misc
{
    public class SunkenCaptainPaintingTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
			TileObjectData.newTile.StyleHorizontal = true;
			TileObjectData.newTile.StyleWrapLimit = 36;
			TileObjectData.addTile(Type);
            DustType = DustID.WoodFurniture;
			AddMapEntry(new Color(124, 99, 90), Language.GetText("MapObject.Painting"));
            AnimationFrameHeight = 54;
        }
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (Main.dayTime)
            {
                frameCounter = 0;
                frame = 0;
                return;
            }
            if (++frameCounter > 180)
            {
                frame = 1;
                if (frameCounter >= 190 + Main.rand.Next(0, 41))
                {
                    frame = Main.rand.Next(4) + 2;
                    frameCounter = 0;
                }
                return;
            }
        }
        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;

            int height = tile.TileFrameY % AnimationFrameHeight >= 16 ? 18 : 16;
            int animate = Main.tileFrame[Type] * AnimationFrameHeight;
            Rectangle frame = new(tile.TileFrameX, tile.TileFrameY + animate, 16, height);
            Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, new Vector2((i * 16) - (int)Main.screenPosition.X, (j * 16) - (int)Main.screenPosition.Y) + zero, frame, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
        }
    }
}