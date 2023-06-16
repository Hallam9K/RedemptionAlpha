using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Items.Placeable.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Walls
{
	public class MossyLabWallTile : ModWall
	{
		public override void SetStaticDefaults()
        {
            RedeTileHelper.CannotTeleportInFront[Type] = true;
            Main.wallHouse[Type] = false;
			AddMapEntry(new Color(8, 64, 39));
            RegisterItemDrop(ModContent.ItemType<LabPlating>());
            HitSound = SoundID.Grass;
        }
        public override bool CanExplode(int i, int j) => false;
        public override void KillWall(int i, int j, ref bool fail) => fail = true;
    }
    public class MossyLabWall : PlaceholderTile
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Mossy Lab Wall (Full) (Unsafe)");
            // Tooltip.SetDefault("[c/ff0000:Unbreakable]");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<MossyLabWallTile>();
        }
    }
}