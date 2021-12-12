using Redemption.Tiles.Furniture.AncientWood;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.AncientWood
{
    public class AncientWoodBed : ModItem
	{
		public override void SetStaticDefaults()
		{
            Tooltip.SetDefault("'Uncomfortable and feels rough...'");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<AncientWoodBedTile>(), 0);
			Item.width = 34;
			Item.height = 18;
			Item.maxStack = 99;
			Item.value = 2000;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.AncientWood>(), 15)
				.AddIngredient(ItemID.Silk, 5)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}