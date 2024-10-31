using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Terrarium
{
    public class KabucraCageTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileTable[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
            TileObjectData.newTile.UsesCustomCanPlace = true;
            TileObjectData.newTile.Width = 6;
            TileObjectData.newTile.Height = 3;
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16, 16 };
            TileObjectData.newTile.CoordinateWidth = 16;
            TileObjectData.newTile.CoordinatePadding = 2;
            TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.Table | AnchorType.SolidTile | AnchorType.SolidWithTop | AnchorType.Platform | AnchorType.Table, TileObjectData.newTile.Width, 0);
            TileObjectData.newTile.Origin = new Point16(3, 2);
            TileObjectData.addTile(Type);

            DustType = DustID.Glass;
            AnimationFrameHeight = 54;

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(200, 200, 200), name);
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (frame is 0) // Hiding Right
            {
                if (++frameCounter >= 10)
                {
                    frameCounter = 0;
                    if (Main.rand.NextBool(50))
                        frame = 1; // Stand Right
                }
            }
            else if (frame >= 1 && frame <= 5) // Stand Right
            {
                if (++frameCounter >= 7)
                {
                    frameCounter = 0;
                    if (++frame > 5)
                    {
                        if (Main.rand.NextBool(30))
                            frame = 6; // Walk
                        else
                            frame = 5;
                    }
                }
            }
            else if (frame >= 6) // Walk
            {
                if (++frameCounter >= 7)
                {
                    frameCounter = 0;
                    if (++frame > 37)
                        frame = 0; // Hiding Right
                }
            }
        }
    }
}