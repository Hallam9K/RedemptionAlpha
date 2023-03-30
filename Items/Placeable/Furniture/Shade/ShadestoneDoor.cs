using Redemption.Tiles.Furniture.Shade;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class ShadestoneDoor : ModItem
	{
		public override void SetStaticDefaults()
		{
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ShadestoneDoorClosed>(), 0);
			Item.width = 20;
			Item.height = 34;
			Item.maxStack = Item.CommonMaxStack;
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