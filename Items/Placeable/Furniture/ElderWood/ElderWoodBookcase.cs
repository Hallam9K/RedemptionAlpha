using Redemption.Tiles.Furniture.ElderWood;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.ElderWood
{
    public class ElderWoodBookcase : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ElderWoodBookcaseTile>(), 0);
			Item.width = 26;
			Item.height = 32;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 60;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.ElderWood>(), 20)
				.AddIngredient(ItemID.Book, 10)
				.AddTile(TileID.Sawmill)
				.Register();
		}
	}
}