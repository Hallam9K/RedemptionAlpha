using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Redemption.Tiles.Furniture.SlayerShip;

namespace Redemption.Items.Placeable.Furniture.SlayerShip
{
    public class SolarPanel : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Solar Panels");
            Item.ResearchUnlockCount = 1;
        }
        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<SolarPanelTile>(), 0);
            Item.width = 32;
            Item.height = 24;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.LightPurple;
		}
    }
}