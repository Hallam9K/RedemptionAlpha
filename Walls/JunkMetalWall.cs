using Microsoft.Xna.Framework;
using Redemption.Globals;
using Redemption.Items.Materials.HM;
using Redemption.Items.Placeable.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Walls
{
    public class JunkMetalWall : ModWall
	{
		public override void SetStaticDefaults()
        {
            RedeTileHelper.CannotTeleportInFront[Type] = true;
            Main.wallHouse[Type] = false;
            RegisterItemDrop(ModContent.ItemType<Cyberscrap>());
            AddMapEntry(new Color(113, 115, 120));
		}
        public override bool CanExplode(int i, int j) => false;
    }
    public class JunkMetalWallItem : PlaceholderTile
    {
        public override string Texture => Redemption.PLACEHOLDER_TEXTURE;
        public override void SetSafeStaticDefaults()
        {
            // DisplayName.SetDefault("Cyberscrap Wall");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.createWall = ModContent.WallType<JunkMetalWall>();
        }
    }
}