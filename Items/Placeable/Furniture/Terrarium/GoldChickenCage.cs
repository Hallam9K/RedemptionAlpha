using Redemption.Items.Critters;
using Redemption.Tiles.Furniture.Terrarium;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Terrarium
{
    public class GoldChickenCage : ModItem
    {
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GoldChickenCageTile>(), 0);
            Item.width = 34;
            Item.height = 32;
            Item.value = Item.sellPrice(0, 10);
            Item.rare = ItemRarityID.Orange;
            Item.maxStack = Item.CommonMaxStack;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Terrarium)
                .AddIngredient<ChickenGoldItem>()
                .Register();
        }
    }
}