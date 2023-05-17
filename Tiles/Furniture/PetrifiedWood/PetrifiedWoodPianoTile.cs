using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.PetrifiedWood
{
    public class PetrifiedWoodPianoTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);

			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Petrified Wood Piano");
			AddMapEntry(new Color(100, 100, 100), name);

			AdjTiles = new int[] { TileID.Pianos };
			DustType = DustID.Ash;
		}
		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	}
}
