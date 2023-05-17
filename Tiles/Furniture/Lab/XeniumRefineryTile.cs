using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Lab
{
    public class XeniumRefineryTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileLighted[Type] = true;
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 4;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(1, 3);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            DustType = DustID.Electric;
            MinPick = 200;
            MineResist = 6f;
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Xenium Refinery");
			AddMapEntry(new Color(54, 193, 59), name);
		}
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.0f;
            g = 0.2f;
            b = 0.0f;
        }
        public override bool CanExplode(int i, int j) => false;
    }
}