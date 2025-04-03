using Redemption.Projectiles.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class PetrifiedWoodWallTile : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            DustType = DustID.Ash;
            AddMapEntry(new Color(48, 44, 42));
        }

        public override void NumDust(int i, int j, bool fail, ref int num)
        {
            num = fail ? 1 : 3;
        }
        public override void Convert(int i, int j, int conversionType)
        {
            if (conversionType == GetInstance<WastelandSolutionConversion>().Type)
                return;
            WorldGen.ConvertWall(i, j, WallID.Wood);
        }
    }
}