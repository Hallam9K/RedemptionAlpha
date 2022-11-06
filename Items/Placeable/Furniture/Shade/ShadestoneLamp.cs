using Redemption.Tiles.Furniture.Shade;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class ShadestoneLamp : ModItem
	{
		public override void SetStaticDefaults()
		{
			SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ShadestoneLampTile>(), 0);
			Item.width = 12;
			Item.height = 34;
			Item.maxStack = 9999;
			Item.value = 100;
			Item.rare = ItemRarityID.Blue; 
        }
        public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient(ItemID.Torch)
				.AddIngredient(ModContent.ItemType<Tiles.Shadestone>(), 3)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}