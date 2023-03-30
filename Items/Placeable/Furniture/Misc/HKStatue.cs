using Redemption.Rarities;
using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class HKStatue : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Statue of the Demigod... ?");
            /* Tooltip.SetDefault("[c/ffea9b:'Spent years waiting for the time,]" +
                "\n[c/ffea9b:I'd come to see the truth behind,]" +
                "\n[c/ffea9b:The darkness that we face in our own lives,]" +
                "\n[c/ffea9b:Cast aside your fear,]" +
                "\n[c/ffea9b:We're strong in numbers,]" +
                "\n[c/ffea9b:Holding back a fate of endless slumber,]" +
                "\n[c/ffea9b:I want to stay to keep you strong,]" +
                "\n[c/ffea9b:Side by side is where we belong...']"
                + "\n[c/ff0000:Unbreakable (500% Pickaxe Power)]"); */
        }

		public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<HKStatueTile>(), 0);
            Item.width = 30;
            Item.height = 44;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<LegendaryRarity>();
            Item.value = Item.sellPrice(5, 0, 0, 0);
        }
    }
}