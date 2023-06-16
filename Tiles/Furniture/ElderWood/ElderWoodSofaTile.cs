using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.ElderWood
{
    public class ElderWoodSofaTile : ModTile
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
			// name.SetDefault("Elder Wood Sofa");
			AddMapEntry(new Color(109, 87, 78), name);

            AddToArray(ref TileID.Sets.RoomNeeds.CountsAsChair);
            AdjTiles = new int[] { TileID.Benches };
			DustType = DustID.t_BorealWood;
		}
		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	}
}
