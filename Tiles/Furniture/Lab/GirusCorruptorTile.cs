using Microsoft.Xna.Framework;
using Redemption.Dusts.Tiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.ObjectData;

namespace Redemption.Tiles.Furniture.Lab
{
    public class GirusCorruptorTile : ModTile
    {
        public override void SetStaticDefaults()
        {
            Main.tileLighted[Type] = true;
            Main.tileFrameImportant[Type] = true;
            TileObjectData.newTile.CopyFrom(TileObjectData.Style2x2);
            TileObjectData.newTile.Height = 2;
            TileObjectData.newTile.Width = 2;
            TileObjectData.newTile.Origin = new Point16(0, 1);
            TileObjectData.newTile.CoordinateHeights = new int[] { 16, 16 };
            TileObjectData.addTile(Type);
            LocalizedText name = CreateMapEntryName();
            // name.SetDefault("Girus Corruptor");
            AddMapEntry(new Color(185, 23, 23), name);
            DustType = ModContent.DustType<LabPlatingDust>();
            AnimationFrameHeight = 36;
            MinPick = 200;
            MineResist = 3f;
        }
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
        public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
        {
            r = 0.4f;
            g = 0.0f;
            b = 0.0f;
        }
    }
}