using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.SlayerShip;

namespace Redemption.Items.Placeable.Furniture.SlayerShip
{
    public class SolarPanel : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Solar Panels");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<SolarPanelTile>(), 0);
            Item.width = 32;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.LightPurple;
		}
    }
}