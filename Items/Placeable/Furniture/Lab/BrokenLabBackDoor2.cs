using Redemption.Items.Placeable.Tiles;
using Redemption.Tiles.Furniture.Lab;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class BrokenLabBackDoor2 : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Broken Laboratory Back Door");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<BrokenLabBackDoor2Tile>(), 0);
            Item.width = 34;
            Item.height = 34;
            Item.maxStack = 9999;
            Item.value = 100;
            Item.rare = ItemRarityID.LightPurple;
		}
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ModContent.ItemType<LabPlating>(), 15)
                .AddIngredient(ItemID.Glass, 4)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}