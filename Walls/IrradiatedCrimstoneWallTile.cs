using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class IrradiatedCrimstoneWallTile : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            WallID.Sets.Crimson[Type] = true;
            DustType = DustID.Ash;
            AddMapEntry(new Color(50, 40, 40));
        }
    }
    public class IrradiatedCrimstoneWallTileSafe : ModWall
    {
        public override string Texture => "Redemption/Walls/IrradiatedCrimstoneWallTile";
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            WallID.Sets.Crimson[Type] = true;
            DustType = DustID.Ash;
            AddMapEntry(new Color(50, 40, 40));
        }
    }
}