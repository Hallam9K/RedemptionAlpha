using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.SlayerShip
{
    public class SlayerShipEngineTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 4;
            TileObjectData.newTile.Height = 6;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16, 16, 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            DustType = DustID.Electric;
            MinPick = 200;
            MineResist = 10f;
            AddMapEntry(new Color(189, 191, 200));
            AnimationFrameHeight = 108;
        }
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 2)
            {
                frameCounter = 0;
                frame++;
                if (frame > 3)
                {
                    frame = 0;
                }
            }
        }
        public override bool CanExplode(int i, int j) => false;
    }
}