using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.SlayerShip
{
    public class SlayerHullPlatingTile : ModTile
	{
        public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);
            DustType = 7;
            MinPick = 200;
            MineResist = 7f;
			AddMapEntry(new Color(189, 191, 200));
        }
    }
}