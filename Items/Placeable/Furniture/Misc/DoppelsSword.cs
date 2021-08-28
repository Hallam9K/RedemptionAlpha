using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class DoppelsSword : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Steel Sword Fragment");
        }

		public override void SetDefaults()
		{
			Item.width = 46;
			Item.height = 50;
			Item.maxStack = 99;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.rare = ItemRarityID.LightRed;
			Item.consumable = true;
			Item.value = Item.sellPrice(0, 0, 50, 0);
			Item.createTile = ModContent.TileType<DoppelsSwordTile>();
		}
	}
}