using Redemption.Tiles.Furniture.PetrifiedWood;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.PetrifiedWood
{
    public class PetrifiedWoodDoor : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<PetrifiedWoodDoorClosed>(), 0);
			Item.width = 20;
			Item.height = 34;
			Item.maxStack = 99;
			Item.value = 150;
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.PetrifiedWood>(), 6)
				.AddTile(TileID.WorkBenches)
				.Register();
		}
	}
}