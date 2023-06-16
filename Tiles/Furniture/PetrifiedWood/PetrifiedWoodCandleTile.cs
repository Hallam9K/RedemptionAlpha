using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ID;
using Terraria.ObjectData;
using Terraria.Enums;
using Redemption.Items.Placeable.Furniture.PetrifiedWood;

namespace Redemption.Tiles.Furniture.PetrifiedWood
{
    public class PetrifiedWoodCandleTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = true;
            Main.tileLighted[Type] = true;
            TileID.Sets.DisableSmartCursor[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.StyleOnTable1x1);
            TileObjectData.newTile.WaterDeath = true;
            TileObjectData.newTile.WaterPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.LavaPlacement = LiquidPlacement.NotAllowed;
            TileObjectData.newTile.CoordinateHeights = new int[] { 20 };
            TileObjectData.newTile.StyleLineSkip = 2;
            TileObjectData.addTile(Type);
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Petrified Wood Candle");
            AddMapEntry(new Color(100, 100, 100), name);
            RegisterItemDrop(ModContent.ItemType<PetrifiedWoodCandle>());
            AdjTiles = new int[]{ TileID.Candles };
            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTorch);
            DustType = DustID.Ash;
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
            player.cursorItemIconID = ModContent.ItemType<PetrifiedWoodCandle>();
        }

        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            Tile tile = Framing.GetTileSafely(i, j);
            if (tile.TileFrameX < 18)
            {
                r = 0.7f;
                g = 0.7f;
                b = 0.7f;
            }
        }
    }
}