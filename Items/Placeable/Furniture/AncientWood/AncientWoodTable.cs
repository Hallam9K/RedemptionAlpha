using Redemption.Tiles.Furniture.AncientWood;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.AncientWood
{
    public class AncientWoodTable : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<AncientWoodTableTile>());
			Item.width = 38;
			Item.height = 24;
			Item.maxStack = 99;
			Item.value = 500;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.AncientWood>(), 8)
				.AddTile(TileID.WorkBenches)
				.Register();
        }
	}
}