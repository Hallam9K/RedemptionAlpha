using Redemption.Items.Materials.HM;
using Redemption.Tiles.Bars;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
	public class StarliteBar : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Starlite Bar");
			Tooltip.SetDefault("'Man-made metal that was made a long time ago, but recipe was lost for hundreds of years...'");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 25;
        }
		public override void SetDefaults()
		{
			Item.width = 30;
			Item.height = 24;
			Item.maxStack = 99;
			Item.value = Item.sellPrice(0, 0, 10, 0);
			Item.rare = ItemRarityID.Lime;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = 1;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<StarliteBarTile>();
		}
		public override void AddRecipes()
		{
			CreateRecipe()
			.AddIngredient(ModContent.ItemType<Starlite>(), 2)
			.AddTile(TileID.AdamantiteForge)
			.Register();
		}
	}
}
