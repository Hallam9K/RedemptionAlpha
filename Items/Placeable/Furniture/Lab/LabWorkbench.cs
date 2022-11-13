using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Furniture.Lab;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LabWorkbench : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laboratory Work Bench");
            SacrificeTotal = 2;
        }
        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<LabWorkbenchTile>(), 0);
            Item.width = 16;
            Item.height = 18;
            Item.maxStack = 9999;
            Item.value = 500;
            Item.rare = ItemRarityID.LightPurple;
		}
        public override void AddRecipes()
        {
            CreateRecipe()
               .AddIngredient(ModContent.ItemType<LabPlating>(), 10)
               .Register();
        }
    }
}