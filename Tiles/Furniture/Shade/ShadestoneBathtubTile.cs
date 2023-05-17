using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Shade
{
    public class ShadestoneBathtubTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style4x2);
			TileObjectData.newTile.Origin = new Point16(1, 1);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);

			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Shadestone Bathtub");
			AddMapEntry(new Color(59, 61, 87), name);

			DustType = ModContent.DustType<ShadestoneDust>();
			AdjTiles = new int[] { TileID.Bathtubs };
		}

		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	}
}