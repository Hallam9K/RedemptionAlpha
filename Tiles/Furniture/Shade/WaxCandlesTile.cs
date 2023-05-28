using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Furniture.Shade;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Shade
{
    public class WaxCandlesTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style1x2);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.RandomStyleRange = 5;
            TileObjectData.newTile.DrawYOffset = 2;
            TileObjectData.newTile.StyleLineSkip = 5;
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Wax Candles");
            AddMapEntry(new Color(141, 132, 172), name);
            AdjTiles = new int[] { TileID.Candles };
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            RegisterItemDrop(ModContent.ItemType<WaxCandles>());
            DustType = ModContent.DustType<MaskDust>();
            AnimationFrameHeight = 36;
        }
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 4)
            {
                frameCounter = 0;
                frame++;
                if (frame > 3)
                    frame = 0;
            }
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.4f;
            g = 0.4f;
            b = 0.5f;
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
                zero = Vector2.Zero;
            int height = tile.TileFrameY % AnimationFrameHeight >= 16 ? 18 : 16;
            int animate = Main.tileFrame[Type] * AnimationFrameHeight;

            Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Rectangle frame = new(tile.TileFrameX, tile.TileFrameY + animate, 16, height);
            ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (uint)i);
            Color color = new(100, 100, 100, 0);
            for (int k = 0; k < 4; k++)
            {
                float xx = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
                float yy = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;
                Vector2 drawPosition = new Vector2(i * 16 - (int)Main.screenPosition.X + xx, j * 16 - (int)Main.screenPosition.Y + yy) + zero;
                spriteBatch.Draw(texture, drawPosition, frame, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
