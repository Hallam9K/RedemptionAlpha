using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Redemption.Tiles.Furniture.Silverwood;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Silverwood
{
    public class AnvilLog : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ancient Anvil");
            Tooltip.SetDefault("Functions as a normal anvil");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AnvilLogTile>(), 0);
            Item.width = 32;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 250;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<AncientAlloy>(5)
                .AddIngredient<Tiles.Silverwood>(4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
