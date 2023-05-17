using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Misc
{
    public class SlayerMedalTile : ModTile
	{
		public override void SetStaticDefaults()
		{
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileObjectData.newTile.Width = 1;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);
            DustType = DustID.WoodFurniture;
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Medal");
			AddMapEntry(new Color(208, 191, 80), name);
		}
    }
}