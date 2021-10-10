using Redemption.Tiles.Tiles;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class SlayerShipPanel : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Slayer's Ship Panel");
            Tooltip.SetDefault("[c/ff0000:Unbreakable]");
        }

		public override void SetDefaults()
		{
			Item.width = 16;
			Item.height = 16;
			Item.maxStack = 999;
			Item.useTurn = true;
			Item.autoReuse = true;
			Item.useAnimation = 15;
			Item.useTime = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.consumable = true;
			Item.createTile = ModContent.TileType<SlayerShipPanelTile>();
		}
    }
}
