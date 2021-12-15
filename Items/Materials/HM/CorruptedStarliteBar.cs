using Redemption.Tiles.Bars;
using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
	public class CorruptedStarliteBar : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Corrupted Starlite Bar");
			Tooltip.SetDefault("'The star's life has ended...'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 24;
			Item.maxStack = 99;
			Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Red;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<CorruptedStarliteBarTile>();
		}
		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(ModContent.ItemType<StarliteBar>(), 1)
			.AddIngredient(ItemID.Ectoplasm, 1)
			.AddTile(ModContent.TileType<GirusCorruptorTile>())
			.Register();
		}
	}
}
