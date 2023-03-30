using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class Asteroid : ModItem
    {
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 100;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AsteroidTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 100;
        }
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Meteorite)
                .AddCondition(Condition.NearWater)
                .Register();
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<AsteroidWall>(), 4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
