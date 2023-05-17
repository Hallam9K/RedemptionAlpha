using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Lab
{
    public class LabCrate2Tile : ModTile
	{
		public override void SetStaticDefaults()
		{
            Main.tileFrameImportant[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileTable[Type] = true;

            // Placement
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.CoordinateHeights = new int[2] { 16, 18 };
            TileObjectData.newTile.StyleHorizontal = true;
            TileObjectData.addTile(Type);

            DustType = ModContent.DustType<LabPlatingDust>();
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Reinforced Laboratory Crate");
			AddMapEntry(new Color(189, 191, 200), name);
        }
        public override bool CreateDust(int i, int j, ref int type) => false;
    }
}