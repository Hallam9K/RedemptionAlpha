using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class IrradiatedSnowWallTile : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            DustType = DustID.Ash;
            AddMapEntry(new Color(100, 109, 100));
        }
    }
    public class IrradiatedSnowWallTileSafe : ModWall
    {
        public override string Texture => "Redemption/Walls/IrradiatedSnowWallTile";
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            DustType = DustID.Ash;
            AddMapEntry(new Color(100, 109, 100));
        }
    }
}