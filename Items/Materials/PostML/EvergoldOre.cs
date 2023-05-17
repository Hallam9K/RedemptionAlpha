using Redemption.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PostML
{
    public class EvergoldOre : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Evergold Nugget");
            Tooltip.SetDefault("'Eternal gold'");
        }

		public override void SetDefaults()
        {
            //Item.DefaultToPlaceableTile(ModContent.TileType<AncientSlateGemTile>(), 0);
            Item.width = 24;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 0, 25, 0);
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
    }
}