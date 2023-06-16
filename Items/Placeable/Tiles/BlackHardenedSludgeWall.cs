using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Redemption.Walls;

namespace Redemption.Items.Placeable.Tiles
{
    public class BlackHardenedSludgeWall : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("[c/ff0000:Unbreakable]");
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
        }

        public override void SetDefaults()
		{
            Item.DefaultToPlaceableWall((ushort)ModContent.WallType<BlackHardenedSludgeWallTile>());
            Item.width = 24;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 0, 1, 0);
            Item.rare = ItemRarityID.Purple;
		}
    }
}