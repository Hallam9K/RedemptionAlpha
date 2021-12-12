using Redemption.Tiles.Furniture.AncientWood;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.AncientWood
{
    public class AncientWoodLantern : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<AncientWoodLanternTile>());
			Item.width = 16;
			Item.height = 26;
			Item.maxStack = 999;
			Item.value = 30;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.AncientWood>(), 6)
				.AddIngredient(ItemID.Torch)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}