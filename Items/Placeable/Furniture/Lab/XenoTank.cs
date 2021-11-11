using Redemption.Tiles.Furniture.Lab;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Lab
{
	public class XenoTank : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Xenium Refinery");
			Tooltip.SetDefault("Used to craft Xenium\nFound in the Abandoned Lab");
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
		public override void SetDefaults()
		{
			Item.width = 54;
			Item.height = 64;
			Item.maxStack = 99;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = 1;
			Item.consumable = true;
			Item.value = Item.sellPrice(0, 10, 0, 0);
			Item.rare = ItemRarityID.Purple;
			Item.createTile = ModContent.TileType<XenoTank1>();
			Item.placeStyle = 0;
		}
	}
}
