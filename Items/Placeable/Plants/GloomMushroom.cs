using Redemption.Items.Materials.PreHM;
using Redemption.Tiles.Plants;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Plants
{
    public class GloomMushroom : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(TileType<GloomShroomFoliage>());
            Item.width = 18;
            Item.height = 18;
            Item.maxStack = Item.CommonMaxStack;
        }
        public override void AddRecipes()
        {
            CreateRecipe(10)
                .AddIngredient(ItemID.GlowingMushroom, 10)
                .AddIngredient<LostSoul>()
                .AddCondition(Condition.InGraveyard)
                .Register();
            CreateRecipe(3)
                .AddIngredient<GloomMushroomBig>()
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
