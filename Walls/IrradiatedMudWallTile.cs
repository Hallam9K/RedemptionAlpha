using Microsoft.Xna.Framework;
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
    }
    public class IrradiatedMudWall : PlaceholderTile
    {
        public override string Texture => "Redemption/Tiles/Placeholder/IrradiatedMudWall";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<IrradiatedMudWallTile>();
        }
    }
}