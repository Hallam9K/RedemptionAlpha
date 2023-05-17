using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.ElderWood
{
    public class ElderWoodSinkTile : ModTile
	{
		public override void SetStaticDefaults()
		{
            TileID.Sets.CountsAsWaterSource[Type] = true;
            Main.tileFrameImportant[Type] = true;
			Main.tileNoAttach[Type] = true;
			Main.tileLavaDeath[Type] = true;
			TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
			TileObjectData.newTile.CoordinateHeights = new[] { 16, 18 };
			TileObjectData.newTile.Origin = new Point16(0, 1);
			TileObjectData.newTile.DrawYOffset = 2;
			TileObjectData.addTile(Type);

			AddMapEntry(new Color(109, 87, 78), Language.GetText("MapObject.Sink"));

			AdjTiles = new int[] { TileID.Sinks };
			DustType = DustID.t_BorealWood;
		}
		public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
	}
}
