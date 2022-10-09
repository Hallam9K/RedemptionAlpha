using Redemption.Tiles.Furniture.Shade;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class ShadestoneDoor : ModItem
	{
		public override void SetStaticDefaults()
		{
			SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ShadestoneDoorClosed>(), 0);
			Item.width = 20;
			Item.height = 34;
			Item.maxStack = 9999;
			Item.value = 150;
			Item.rare = ItemRarityID.Blue;
        }
        public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.Shadestone>(), 6)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}