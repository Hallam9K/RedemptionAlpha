using Redemption.Tiles.Furniture.Shade;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class ShadestoneDresser : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ShadestoneDresserTile>(), 0);
			Item.width = 34;
			Item.height = 20;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 60;
			Item.rare = ItemRarityID.Blue; 
        }
        public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Tiles.Shadestone>(), 16)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}