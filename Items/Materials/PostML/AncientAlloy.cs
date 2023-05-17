using Redemption.Rarities;
using Redemption.Tiles.Bars;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PostML
{
    public class AncientAlloy : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AncientAlloyTile>(), 0);
            Item.width = 30;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 1, 40, 0);
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
    }
}