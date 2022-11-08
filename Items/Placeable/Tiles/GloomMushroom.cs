using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Redemption.Tiles.Tiles;
using Redemption.Items.Materials.PreHM;

namespace Redemption.Items.Placeable.Tiles
{
    public class GloomMushroom : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 100;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<GloomMushroomTile>(), 0);
            Item.width = 18;
            Item.height = 18;
            Item.maxStack = 9999;
        }
        public override void AddRecipes()
        {
            CreateRecipe(10)
                .AddIngredient(ItemID.GlowingMushroom, 10)
                .AddIngredient<LostSoul>()
                .AddCondition(Recipe.Condition.InGraveyardBiome)
                .Register();
        }
    }
}
