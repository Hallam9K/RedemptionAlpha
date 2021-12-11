using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Furniture.AncientWood;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.AncientWood
{
	public class AncientWoodWorkbenchTile : ModTile
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

			DustType = DustID.t_BorealWood;
			AdjTiles = new int[] { TileID.WorkBenches };

			// Placement
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
			TileObjectData.newTile.CoordinateHeights = new[] { 18 };
			TileObjectData.addTile(Type);

			AddToArray(ref TileID.Sets.RoomNeeds.CountsAsTable);

			// Etc
			ModTranslation name = CreateMapEntryName();
			name.SetDefault("Ancient Wood Work Bench");
			AddMapEntry(new Color(109, 87, 78), name);
		}

		public override void NumDust(int x, int y, bool fail, ref int num) => num = fail ? 1 : 3;

		public override void KillMultiTile(int x, int y, int frameX, int frameY)
		{
			Item.NewItem(x * 16, y * 16, 32, 16, ModContent.ItemType<AncientWoodWorkBench>());
		}
	}
}