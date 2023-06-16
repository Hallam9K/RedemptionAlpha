using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;
using Terraria.ID;

namespace Redemption.Tiles.Furniture.Lab
{
    public class LabWallFanTile : ModTile
	{
		public override void SetStaticDefaults()
		{
			Main.tileFrameImportant[Type] = true;
			Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            Main.tileTable[Type] = false;
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
            MineResist = 4f;
			LocalizedText name = CreateMapEntryName();
			// name.SetDefault("Laboratory Wall Fan");
			AddMapEntry(new Color(56, 55, 61), name);
            AnimationFrameHeight = 36;
        }
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 4)
            {
                frameCounter = 0;
                frame++;
                if (frame > 5)
                    frame = 0;
            }
        }
        public override bool CanExplode(int i, int j) => false;
    }
}