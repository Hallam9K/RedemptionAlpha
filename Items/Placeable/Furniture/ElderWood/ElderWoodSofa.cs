using Redemption.Tiles.Furniture.ElderWood;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.ElderWood
{
    public class ElderWoodSofa : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ElderWoodSofaTile>(), 0);
			Item.width = 38;
			Item.height = 24;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 60;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.ElderWood>(), 5)
				.AddIngredient(ItemID.Silk, 2)
				.AddTile(TileID.Sawmill)
				.Register();
		}
	}
}