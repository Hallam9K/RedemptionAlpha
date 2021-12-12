using Redemption.Tiles.Furniture.AncientWood;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.AncientWood
{
    public class AncientWoodChandelier : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<AncientWoodChandelierTile>());
			Item.width = 34;
			Item.height = 34;
			Item.maxStack = 99;
			Item.value = 600;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.AncientWood>(), 4)
				.AddIngredient(ItemID.Torch, 4)
				.AddIngredient(ItemID.Chain)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}