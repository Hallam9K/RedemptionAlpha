using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class LabPlatingWallTileUnsafe : ModWall
    {
        public override void SetStaticDefaults()
        {
            RedeTileHelper.CannotTeleportInFront[Type] = true;
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(53, 54, 60));
        }
        public override bool CanExplode(int i, int j) => false;
        public override void KillWall(int i, int j, ref bool fail) => fail = true;
    }
    public class LabPlatingWallTileUnsafe2 : ModWall
    {
        public override string Texture => "Redemption/Walls/LabPlatingWallTileUnsafe";
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            AddMapEntry(new Color(53, 54, 60));
        }
        public override bool CanExplode(int i, int j) => false;
    }
    public class LabPlatingWallUnsafe : PlaceholderTile
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Lab Wall (Unsafe)");
            // Tooltip.SetDefault("[c/ff0000:Unbreakable]");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<LabPlatingWallTileUnsafe>();
        }
    }
}