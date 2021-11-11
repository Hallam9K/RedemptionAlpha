using Redemption.Items.Materials.HM;
using Redemption.Items.Materials.PreHM;
using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PostML
{
	public class RawXenium : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Raw Xenium");
			Tooltip.SetDefault("Use a Xenium Refinery to craft Xenium Bars");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 50;
        }
		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.maxStack = 999;
			Item.value = Item.sellPrice(0, 0, 15, 0);
			Item.rare = ItemRarityID.Purple;
        }
		public override void AddRecipes()
		{
			CreateRecipe(2)
			.AddIngredient(ModContent.ItemType<XenomiteShard>(), 2)
			.AddIngredient(ModContent.ItemType<Starlite>(), 2)
			.AddIngredient(ItemID.LunarOre, 1)
			.AddTile(ModContent.TileType<XenoTank1>())
			.Register();
		}
	}
}
