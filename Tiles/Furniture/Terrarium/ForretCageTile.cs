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
    public class ForretCageTile : ModTile
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
            if (frame <= 21) // Frolic
            {
                if (++frameCounter >= 7)
                {
                    frameCounter = 0;
                    if (++frame > 21)
                        frame = 22; // Stand Left
                }
            }
            else if (frame == 22) // Stand Left
            {
                if (++frameCounter >= 10)
                {
                    frameCounter = 0;
                    if (Main.rand.NextBool(30))
                    {
                        switch (Main.rand.Next(2))
                        {
                            case 0:
                                frame = 23; // Sleep
                                break;
                            case 1:
                                frame = 0; // Frolic
                                break;
                        }
                    }
                }
            }
            else if (frame >= 23 && frame <= 25) // Sleep
            {
                if (++frameCounter >= 7)
                {
                    frameCounter = 0;
                    if (++frame > 25)
                    {
                        if (Main.rand.NextBool(60))
                            frame = 26; // Wake
                        else
                            frame = 25;
                    }
                }
            }
            else if (frame >= 26) // Wake
            {
                if (++frameCounter >= 7)
                {
                    frameCounter = 0;
                    if (++frame > 27)
                        frame = 22; // Stand Left
                }
            }
        }
    }
}