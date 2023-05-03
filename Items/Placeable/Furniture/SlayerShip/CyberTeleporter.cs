using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.SlayerShip;

namespace Redemption.Items.Placeable.Furniture.SlayerShip
{
    public class CyberTeleporter : ModItem
	{
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<CyberTeleporterTile>(), 0);
            Item.width = 36;
            Item.height = 14;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.LightPurple;
		}
    }
}