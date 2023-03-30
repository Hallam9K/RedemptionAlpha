using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Furniture.Lab;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LabChair : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Laboratory Chair");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<LabChairTile>(), 0);
            Item.width = 16;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 400;
            Item.rare = ItemRarityID.LightPurple;
		}
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<LabPlating>(), 4)
                .AddIngredient(ItemID.Silk, 2)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}