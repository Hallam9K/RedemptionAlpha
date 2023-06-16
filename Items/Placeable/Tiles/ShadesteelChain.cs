using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria.ModLoader;
using Terraria;

namespace Redemption.Items.Placeable.Tiles
{
    public class ShadesteelChain : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("Can be climbed on");
            Item.ResearchUnlockCount = 100;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<ShadesteelChainTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 0, 0, 40);
            Item.rare = ModContent.RarityType<SoullessRarity>();
            Item.tileBoost += 3;
        }
    }
}
