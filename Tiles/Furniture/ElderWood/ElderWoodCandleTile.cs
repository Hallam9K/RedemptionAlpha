using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Items.Placeable.Furniture.ElderWood;
using Terraria;
using Terraria.Audio;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.ElderWood
{
    public class ElderWoodCandleTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 18 };
            TileObjectData.newTile.WaterDeath = true;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.addTile(Type);
            AddMapEntry(new Color(109, 87, 78), Language.GetText("MapObject.Candle"));
            AdjTiles = new int[] { TileID.Candles };
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            DustType = DustID.t_BorealWood;
        }
        public override void HitWire(int i, int j)
        {
            if (Main.tile[i, j].TileFrameX >= 18)
                Main.tile[i, j].TileFrameX -= 18;
            else
                Main.tile[i, j].TileFrameX += 18;
        }
        public override bool RightClick(int i, int j)
        {
            SoundEngine.PlaySound(SoundID.Mech, new Vector2(i * 16, j * 16));
            if (Main.tile[i, j].TileFrameX >= 18)
                Main.tile[i, j].TileFrameX -= 18;
            else
                Main.tile[i, j].TileFrameX += 18;
            return true;
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override void MouseOver(int i, int j)
        {
            Player player = Main.LocalPlayer;
            player.noThrow = 2;
            player.cursorItemIconEnabled = true;
            player.cursorItemIconID = ModContent.ItemType<ElderWoodCandle>();
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX < 18)
            {
                r = 0.8f;
                g = 0.6f;
                b = 0.4f;
            }
        }

        public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (!TileDrawing.IsVisible(tile))
                return;
            Vector2 zero = new(Main.offScreenRange, Main.offScreenRange);
            if (Main.drawToScreen)
            {
                zero = Vector2.Zero;
            }
            int height = tile.TileFrameY == 36 ? 18 : 16;
            ulong randSeed = Main.TileFrameSeed ^ (ulong)((long)j << 32 | (uint)i);
            Color color = new(100, 100, 100, 0);
            for (int k = 0; k < 7; k++)
            {
                float xx = Utils.RandomInt(ref randSeed, -10, 11) * 0.15f;
                float yy = Utils.RandomInt(ref randSeed, -10, 1) * 0.35f;

                Main.spriteBatch.Draw(ModContent.Request<Texture2D>(Texture + "_Glow").Value, new Vector2((i * 16) - (int)Main.screenPosition.X + xx, (j * 16) - (int)Main.screenPosition.Y + yy) + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, height), color, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0f);
            }
        }
    }
}