using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class IrradiatedEbonstoneWallTile : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            WallID.Sets.Corrupt[Type] = true;
            DustType = DustID.Ash;
            AddMapEntry(new Color(31, 30, 33));
        }
    }
    public class IrradiatedEbonstoneWallTileSafe : ModWall
    {
        public override string Texture => "Redemption/Walls/IrradiatedEbonstoneWallTile";
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            WallID.Sets.Corrupt[Type] = true;
            DustType = DustID.Ash;
            AddMapEntry(new Color(31, 30, 33));
        }
    }
}