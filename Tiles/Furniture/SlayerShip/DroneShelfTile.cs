using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.SlayerShip
{
    public class DroneShelfTile : ModTile
	{
		public override void SetStaticDefaults()
		{
            Main.tileSolidTop[Type] = true;
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.StyleWrapLimit = 2;
            TileObjectData.newTile.StyleMultiplier = 2;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(1, 2);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Drone Shelves");
            AddMapEntry(new Color(151, 153, 160), name);
            DustType = ModContent.DustType<LabPlatingDust>();
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override bool CanExplode(int i, int j) => false;
    }
}