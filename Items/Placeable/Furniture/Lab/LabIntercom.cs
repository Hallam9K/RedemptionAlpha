using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Redemption.Tiles.Furniture.Lab;
using Redemption.Items.Placeable.Tiles;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LabIntercom : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Laboratory Intercom");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabIntercomTile>(), 0);
            Item.width = 24;
            Item.height = 28;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 100;
            Item.rare = ItemRarityID.LightPurple;
		}
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<LabPlating>(), 8)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}