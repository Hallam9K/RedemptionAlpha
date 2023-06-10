using Redemption.Tiles.Furniture.ElderWood;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Redemption.Items.Placeable.Furniture.ElderWood
{
    public class ElderWoodChair : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ElderWoodChairTile>(), 0);
			Item.width = 16;
			Item.height = 34;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 150;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.ElderWood>(), 4)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}
