using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Redemption.Walls;

namespace Redemption.Items.Placeable.Tiles
{
    public class SuperHardenedSludgeWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Super Hardened Sludge Wall");
            Tooltip.SetDefault("[c/ff0000:Unbreakable]");
        }

        public override void SetDefaults()
		{
            Item.DefaultToPlacableWall((ushort)ModContent.WallType<SuperHardenedSludgeWallTile>());
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = 999;
            Item.value = Item.buyPrice(0, 0, 1, 0);
            Item.rare = ItemRarityID.Purple;
		}
    }
}