using Microsoft.Xna.Framework;
using Redemption.NPCs.Lab;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Lab
{
    public class WideLabConsoleTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileLavaDeath[Type] = false;
            Main.tileNoAttach[Type] = true;
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.SolidSide, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);
            DustType = 7;
            MinPick = 500;
            MineResist = 3f;
            ModTranslation name = CreateMapEntryName();
            name.SetDefault("Wide Laboratory Console");
            AddMapEntry(new Color(0, 187, 240));
            AnimationFrameHeight = 36;
        }
        public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            frameCounter++;
            if (frameCounter > 4)
            {
                frameCounter = 0;
                frame++;
                if (frame > 1)
                    frame = 0;
            }
        }
        public override bool CanExplode(int i, int j) => false;
    }
}