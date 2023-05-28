using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts.Tiles;
using Redemption.Items.Placeable.Furniture.Shade;

namespace Redemption.Tiles.Furniture.Shade
{
    public class ShadestoneChandelierTile : ModTile
	{
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
            TileObjectData.newTile.Origin = new Point16(1, 0);
            TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.SolidTile | AnchorType.SolidSide, 1, 1);
            TileObjectData.newTile.AnchorBottom = AnchorData.Empty;
            TileObjectData.newTile.LavaDeath = true;
            TileObjectData.newTile.WaterDeath = true;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.StyleWrapLimit = 37;
            TileObjectData.newTile.StyleHorizontal = false;
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(59, 61, 87), Language.GetText("MapObject.Chandelier"));
            AdjTiles = new int[] { TileID.Chandeliers };
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            RegisterItemDrop(ModContent.ItemType<ShadestoneChandelier>());
            DustType = ModContent.DustType<ShadestoneDust>();
            AnimationFrameHeight = 54;
        }
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
        public override void HitWire(int i, int j)
        {
            int left = i - Main.tile[i, j].TileFrameX / 18 % 3;
            int top = j - Main.tile[i, j].TileFrameY / 18 % 3;
            for (int x = left; x < left + 3; x++)
            {
                for (int y = top; y < top + 3; y++)
                {

                    if (Main.tile[x, y].TileFrameX >= 54)
                        Main.tile[x, y].TileFrameX -= 54;
                    else
                        Main.tile[x, y].TileFrameX += 54;
                }
            }
            if (Wiring.running)
            {
                Wiring.SkipWire(left, top);
                Wiring.SkipWire(left, top + 1);
                Wiring.SkipWire(left + 1, top);
                Wiring.SkipWire(left + 1, top + 1);
            }
            NetMessage.SendTileSquare(-1, left, top + 1, 2);
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX < 36)
            {
                r = 0.7f;
                g = 0.7f;
                b = 0.8f;
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

            Texture2D texture = ModContent.Request<Texture2D>(Texture + "_Glow").Value;
            Rectangle frame = new(tile.TileFrameX, tile.TileFrameY + animate, 16, height);
            ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (uint)i);
            Color color = new(100, 100, 100, 0);
            for (int k = 0; k < 7; k++)
            {
                float xx = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
                float yy = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;
                Vector2 drawPosition = new Vector2(i * 16 - (int)Main.screenPosition.X + xx, j * 16 - (int)Main.screenPosition.Y + yy) + zero;
                spriteBatch.Draw(texture, drawPosition, frame, color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}
