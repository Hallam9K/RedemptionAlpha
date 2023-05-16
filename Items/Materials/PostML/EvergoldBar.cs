using Redemption.Rarities;
using Redemption.Tiles.Bars;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PostML
{
    public class EvergoldBar : ModItem
	{
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("'Eternal gold'");
            SacrificeTotal = 25;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<EvergoldTile>(), 0);
            Item.width = 30;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(0, 1, 60, 0);
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<EvergoldOre>(6)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
