using Redemption.Tiles.Furniture.Shade;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Shade
{
    public class ShadestoneLamp : ModItem
	{
		public override void SetStaticDefaults()
		{
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<ShadestoneLampTile>(), 0);
			Item.width = 12;
			Item.height = 34;
			Item.maxStack = 99;
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