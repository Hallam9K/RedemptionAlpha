using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;

namespace Redemption.Tiles.Furniture.Lab
{
    public class BiohazardSignTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileID.Sets.FramesOnKillWall[Type] = true;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorWall = true;
            TileObjectData.addTile(Type);
            DustType = ModContent.DustType<LabPlatingDust>();
            MinPick = 200;
            MineResist = 5f;
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Biohazard Sign");
			AddMapEntry(new Color(211, 199, 67), name);
		}
        public override bool CanExplode(int i, int j) => false;
    }
}