using Redemption.Tiles.Furniture.Archcloth;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Archcloth
{
    public class ArchclothThrone : ModItem
	{
		public override void SetStaticDefaults()
		{
			SacrificeTotal = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ArchclothThroneTile>(), 0);
			Item.width = 26;
			Item.height = 36;
			Item.maxStack = 9999;
			Item.rare = ItemRarityID.LightRed;
			Item.value = Item.sellPrice(0, 30, 0, 0);
		}

		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ModContent.ItemType<Materials.PreHM.Archcloth>(), 10)
				.AddIngredient(ItemID.PlatinumBar, 30)
				.AddTile(TileID.Anvils)
				.Register();
		}
	}
}