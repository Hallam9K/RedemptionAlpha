using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Furniture.Lab;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class TestTubes : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 2;
        }
        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<TestTubesTile>(), 0);
            Item.width = 16;
            Item.height = 18;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 500;
            Item.rare = ItemRarityID.LightPurple;
		}
        public override void AddRecipes()
        {
            CreateRecipe(2)
               .AddIngredient(ModContent.ItemType<LabPlating>())
               .AddIngredient(ItemID.Glass, 2)
               .AddTile(TileID.WorkBenches)
               .Register();
        }
    }
}