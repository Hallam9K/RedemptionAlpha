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
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 26;
			Item.height = 36;
			Item.maxStack = 99;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.rare = ItemRarityID.LightRed;
			Item.consumable = true;
			Item.value = Item.sellPrice(0, 30, 0, 0);
			Item.createTile = ModContent.TileType<ArchclothThroneTile>();
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