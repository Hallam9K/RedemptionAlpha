using Redemption.Tiles.Furniture.AncientWood;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.AncientWood
{
    public class AncientWoodLamp : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<AncientWoodLampTile>());
			Item.width = 12;
			Item.height = 34;
			Item.maxStack = 99;
			Item.value = 100;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.Torch)
				.AddIngredient(ModContent.ItemType<Tiles.AncientWood>(), 3)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}