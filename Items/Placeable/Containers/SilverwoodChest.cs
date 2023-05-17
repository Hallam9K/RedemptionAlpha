using Redemption.Items.Materials.PostML;
using Redemption.Rarities;
using Redemption.Tiles.Containers;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Containers
{
    public class SilverwoodChest : ModItem
	{
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SilverwoodChestTile>(), 0);
            Item.width = 32;
            Item.height = 30;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 2000;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient<Tiles.Silverwood>(8)
                .AddIngredient<AncientAlloy>(2)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
}