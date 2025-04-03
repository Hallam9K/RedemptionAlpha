using Redemption.Projectiles.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class IrradiatedMudWallTile : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            DustType = DustID.Ash;
            AddMapEntry(new Color(45, 42, 41));
        }
        public override void Convert(int i, int j, int conversionType)
        {
            if (conversionType == GetInstance<WastelandSolutionConversion>().Type)
                return;
            WorldGen.ConvertWall(i, j, WallID.MudUnsafe);
        }
    }
    public class IrradiatedMudWallTileSafe : ModWall
    {
        public override string Texture => "Redemption/Walls/IrradiatedMudWallTile";
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            DustType = DustID.Ash;
            AddMapEntry(new Color(45, 42, 41));
        }
        public override void Convert(int i, int j, int conversionType)
        {
            if (conversionType == GetInstance<WastelandSolutionConversion>().Type)
                return;
            WorldGen.ConvertWall(i, j, WallID.MudWallEcho);
        }
    }
}