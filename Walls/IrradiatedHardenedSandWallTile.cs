using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class IrradiatedHardenedSandWallTile : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            DustType = DustID.Ash;
            AddMapEntry(new Color(86, 62, 54));
        }
    }
    public class IrradiatedHardenedSandWallTileSafe : ModWall
    {
        public override string Texture => "Redemption/Walls/IrradiatedHardenedSandWallTile";
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            DustType = DustID.Ash;
            AddMapEntry(new Color(86, 62, 54));
        }
    }
}