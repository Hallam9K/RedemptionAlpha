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
    public class HazmatBunnyCageTile : ModTile
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
            if (frame is 0 or 1) // Stand Right
            {
                if (++frameCounter >= 10)
                {
                    frameCounter = 0;
                    if (Main.rand.NextBool(30))
                    {
                        switch (Main.rand.Next(3))
                        {
                            case 0:
                                frame = 1; // Blink
                                break;
                            case 1:
                                frame = 2; // Ear Wiggle
                                break;
                            case 2:
                                frame = 4; // Walk Left
                                break;
                        }
                    }
                    else
                        frame = 0;
                }
            }
            else if (frame is 2 or 3) // Ear Wiggle
            {
                if (++frameCounter >= 10)
                {
                    frameCounter = 0;
                    if (++frame > 3)
                        frame = 0;
                }
            }
            else if (frame >= 4 && frame <= 10) // Walk Left
            {
                if (++frameCounter >= 7)
                {
                    frameCounter = 0;
                    if (++frame > 10)
                        frame = 11; // Stand Left
                }
            }
            else if (frame is 11 or 12) // Stand Left
            {
                if (++frameCounter >= 10)
                {
                    frameCounter = 0;
                    if (Main.rand.NextBool(30))
                    {
                        switch (Main.rand.Next(3))
                        {
                            case 0:
                                frame = 12; // Blink
                                break;
                            case 1:
                                frame = 13; // Ear Wiggle
                                break;
                            case 2:
                                frame = 15; // Walk Right
                                break;
                        }
                    }
                    else
                        frame = 11;
                }
            }
            else if (frame is 13 or 14) // Ear Wiggle
            {
                if (++frameCounter >= 10)
                {
                    frameCounter = 0;
                    if (++frame > 14)
                        frame = 11;
                }
            }
            else if (frame >= 15) // Walk Right
            {
                if (++frameCounter >= 7)
                {
                    frameCounter = 0;
                    if (++frame > 21)
                        frame = 0; // Stand Right
                }
            }
        }
    }
}