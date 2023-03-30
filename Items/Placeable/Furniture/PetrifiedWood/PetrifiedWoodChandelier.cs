using Redemption.Tiles.Furniture.PetrifiedWood;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.PetrifiedWood
{
    public class PetrifiedWoodChandelier : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<PetrifiedWoodChandelierTile>());
			Item.width = 34;
			Item.height = 34;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 600;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.PetrifiedWood>(), 4)
				.AddIngredient(ItemID.Torch, 4)
				.AddIngredient(ItemID.Chain)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}