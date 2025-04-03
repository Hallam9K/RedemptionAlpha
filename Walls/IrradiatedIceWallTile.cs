using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class IrradiatedIceWallTile : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            DustType = DustID.Ash;
            AddMapEntry(new Color(65, 97, 66));
        }
    }
    public class IrradiatedIceWallTileSafe : ModWall
    {
        public override string Texture => "Redemption/Walls/IrradiatedIceWallTile";
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = true;
            DustType = DustID.Ash;
            AddMapEntry(new Color(65, 97, 66));
        }
    }
}