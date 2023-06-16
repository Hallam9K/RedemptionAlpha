using Redemption.Tiles.Furniture.ElderWood;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.ElderWood
{
    public class ElderWoodBed : ModItem
	{
		public override void SetStaticDefaults()
		{
            // Tooltip.SetDefault("'Uncomfortable and feels rough...'");

			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ElderWoodBedTile>(), 0);
			Item.width = 34;
			Item.height = 18;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 2000;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.ElderWood>(), 15)
				.AddIngredient(ItemID.Silk, 5)
				.AddTile(TileID.Sawmill)
				.Register();
		}
	}
}