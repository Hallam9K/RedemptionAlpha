using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.DataStructures;
using Terraria.ID;
using Redemption.Items.Placeable.Furniture.Misc;

namespace Redemption.Tiles.Furniture.Misc
{
    public class MudGuardianPaintingTile : ModTile
	{
        public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 5;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(3, 1);
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);
            DustType = DustID.WoodFurniture;
			AddMapEntry(new Color(96, 67, 55));
        }
        public override void KillMultiTile(int i, int j, int frameX, int frameY)
        {
            Item.NewItem(new EntitySource_TileBreak(i, j), i * 16, j * 16, 80, 48, ModContent.ItemType<MudGuardianPainting>());
        }
    }
}