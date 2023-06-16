using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Lab
{
    public class LabWorkbenchTile : ModTile
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

			DustType = ModContent.DustType<LabPlatingDust>();
			AdjTiles = new int[] { TileID.WorkBenches };

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			TileObjectData.newTile.CoordinateHeights = new[] { 16 };
			TileObjectData.addTile(Type);

			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);
			AddMapEntry(new Color(189, 191, 200), Language.GetText("ItemName.WorkBench"));
		}

		public override void NumDust(int x, int y, bool fail, ref int num) => num = fail ? 1 : 3;
	}
}