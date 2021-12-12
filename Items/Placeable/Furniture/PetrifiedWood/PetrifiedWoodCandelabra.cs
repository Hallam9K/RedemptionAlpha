using Redemption.Tiles.Furniture.PetrifiedWood;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.PetrifiedWood
{
    public class PetrifiedWoodCandelabra : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<PetrifiedWoodCandelabraTile>(), 0);
			Item.width = 18;
			Item.height = 32;
			Item.maxStack = 99;
			Item.value = 300;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.PetrifiedWood>(), 5)
				.AddIngredient(ItemID.Torch, 3)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}