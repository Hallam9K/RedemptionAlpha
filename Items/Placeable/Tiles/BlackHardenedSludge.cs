using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class BlackHardenedSludge : ModItem
    {
        public override void SetStaticDefaults()
        {
            ItemID.Sets.DisableAutomaticPlaceableDrop[Type] = true;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<BlackHardenedSludgeTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 0, 2, 0);
            Item.rare = ItemRarityID.LightPurple;
        }
    }
}
