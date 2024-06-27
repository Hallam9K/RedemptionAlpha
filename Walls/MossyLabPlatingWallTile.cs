using Microsoft.Xna.Framework;
using Redemption.Items.Placeable.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class MossyLabPlatingWallTile : ModWall
    {
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            RegisterItemDrop(ModContent.ItemType<LabPlatingWall>());
            AddMapEntry(new Color(53, 54, 60));
        }
        public override bool CanExplode(int i, int j) => false;
        public override void KillWall(int i, int j, ref bool fail) => fail = true;
    }
    public class MossyLabPlatingWallTileUnsafe : ModWall
    {
        public override string Texture => "Redemption/Walls/MossyLabPlatingWallTile";
        public override void SetStaticDefaults()
        {
            Main.wallHouse[Type] = false;
            RegisterItemDrop(ModContent.ItemType<MossyLabPlatingWallUnsafe>());
            AddMapEntry(new Color(53, 54, 60));
        }
        public override bool CanExplode(int i, int j) => false;
    }
    public class MossyLabPlatingWall : PlaceholderTile
    {
        public override string Texture => "Redemption/Items/Placeable/Tiles/MossyLabPlatingWallUnsafe";
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<MossyLabPlatingWallTile>();
        }
    }
}