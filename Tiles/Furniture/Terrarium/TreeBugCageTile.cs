using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Terrarium
{
    public class TreeBugCageTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileFrameImportant[Type] = true;
            Main.tileNoAttach[Type] = true;
            Main.tileLavaDeath[Type] = true;
            Main.tileSolidTop[Type] = true;
            Main.tileTable[Type] = true;

            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Width = 3;
            TileObjectData.newTile.Origin = new Point16(1, 1);
            TileObjectData.newTile.AnchorBottom = new AnchorData(TileObjectData.newTile.AnchorBottom.type, TileObjectData.newTile.Width, 0);
            TileObjectData.addTile(Type);

            DustType = DustID.Glass;
            AnimationFrameHeight = 36;

            LocalizedText name = CreateMapEntryName();
            AddMapEntry(new Color(200, 200, 200), name);
        }
        public override void NumDust(int i, int j, bool fail, ref int num) => num = fail ? 1 : 3;
        public override void AnimateTile(ref int frame, ref int frameCounter)
        {
            if (frame <= 1) // Nom
            {
                if (++frameCounter >= 10)
                {
                    frameCounter = 0;
                    if (++frame > 1)
                    {
                        if (Main.rand.NextBool(30))
                            frame = 2; // Walk Right
                        else
                            frame = 0;
                    }
                }
            }
            else if (frame >= 2 && frame <= 7) // Walk Right
            {
                if (++frameCounter >= 7)
                {
                    frameCounter = 0;
                    if (++frame > 7)
                        frame = 8; // Stand Right
                }
            }
            else if (frame == 8) // Stand Right
            {
                if (++frameCounter >= 10)
                {
                    frameCounter = 0;
                    if (Main.rand.NextBool(30))
                        frame = 9; // Walk Left
                }
            }
            else if (frame >= 9) // Walk Left
            {
                if (++frameCounter >= 7)
                {
                    frameCounter = 0;
                    if (++frame > 12)
                        frame = 0; // Nom
                }
            }
        }
    }
}