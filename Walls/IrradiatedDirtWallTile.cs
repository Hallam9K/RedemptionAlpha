using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class IrradiatedDirtWallTile : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            DustType = DustID.Ash;
            AddMapEntry(new Color(69, 58, 52));
        }
    }
    public class IrradiatedDirtWallTileSafe : ModWall
    {
        public override string Texture => "Redemption/Walls/IrradiatedDirtWallTile";
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            DustType = DustID.Ash;
            AddMapEntry(new Color(69, 58, 52));
        }
    }
}