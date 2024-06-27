using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class IrradiatedSandstoneWallTile : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            DustType = DustID.Ash;
            AddMapEntry(new Color(74, 63, 61));
        }
    }
    public class IrradiatedSandstoneWallTileSafe : ModWall
    {
        public override string Texture => "Redemption/Walls/IrradiatedSandstoneWallTile";
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            DustType = DustID.Ash;
            AddMapEntry(new Color(74, 63, 61));
        }
    }
}