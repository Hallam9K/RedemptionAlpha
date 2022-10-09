using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Furniture.Lab;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LabToilet : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Laboratory Toilet");
            SacrificeTotal = 2;
        }
        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<LabToiletTile>(), 0);
            Item.width = 16;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.value = 1000;
            Item.rare = ItemRarityID.LightPurple;
		}
        public override void AddRecipes()
        {
            CreateRecipe()
               .AddIngredient(ModContent.ItemType<LabPlating>(), 6)
               .AddTile(TileID.WorkBenches)
               .Register();
        }
    }
}