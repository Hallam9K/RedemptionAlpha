using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Furniture.PetrifiedWood;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.PetrifiedWood
{
    public class PetrifiedWoodWorkBenchTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			// Properties
			Main.tileTable[Type] = true;
			Main.tileSolidTop[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			Main.tileFrameImportant[Type] = true;
			TileID.Sets.DisableSmartCursor[Type] = true;
			TileID.Sets.IgnoredByNpcStepUp[Type] = true;

			DustType = DustID.Ash;
			AdjTiles = new int[] { TileID.WorkBenches };

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			TileObjectData.newTile.CoordinateHeights = new[] { 18 };
			TileObjectData.addTile(Type);

			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);

			// Etc
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Petrified Wood Work Bench");
			AddMapEntry(new Color(100, 100, 100), name);
		}

		public override void NumDust(int x, int y, bool fail, ref int num) => num = fail ? 1 : 3;

		public override void KillMultiTile(int x, int y, int frameX, int frameY)
		{
			Item.NewItem(x * 16, y * 16, 32, 16, ModContent.ItemType<PetrifiedWoodWorkBench>());
		}
	}
}