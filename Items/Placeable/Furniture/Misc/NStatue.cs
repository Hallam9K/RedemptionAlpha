using Redemption.Rarities;
using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class NStatue : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Statue of the Protector");
            /* Tooltip.SetDefault("[c/ffea9b:'We're burdened with purpose,]" +
                "\n[c/ffea9b:So here we remain,]" +
                "\n[c/ffea9b:We're incandescent,]" +
                "\n[c/ffea9b:A glorious flame...']" +
                "\n[c/ff0000:Unbreakable (500% Pickaxe Power)]"); */
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<NStatueTile>(), 0);
            Item.width = 30;
            Item.height = 36;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<LegendaryRarity>();
            Item.value = Item.sellPrice(5, 0, 0, 0);
        }
    }
}