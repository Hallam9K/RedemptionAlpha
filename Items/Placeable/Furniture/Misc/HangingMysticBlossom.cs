using Redemption.Items.Placeable.Plants;
using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class HangingMysticBlossom : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<HangingPlantsTile>(), 1);
            Item.width = 16;
            Item.height = 36;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.White;
            Item.value = Item.sellPrice(0, 0, 25);
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.PotSuspended)
                .AddIngredient<AnglonicMysticBlossom>()
                .Register();
        }
    }
}