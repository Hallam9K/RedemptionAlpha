using Redemption.Tiles.Furniture.ElderWood;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Redemption.Items.Placeable.Furniture.ElderWood
{
    public class ElderWoodLantern : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ElderWoodLanternTile>());
			Item.width = 16;
			Item.height = 26;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 30;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.ElderWood>(), 6)
				.AddIngredient(ItemID.Torch)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}