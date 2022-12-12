using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.SlayerShip;
using Terraria.GameContent.Creative;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Materials.HM;

namespace Redemption.Items.Placeable.Furniture.SlayerShip
{
    public class DroneShelf : ModItem
	{
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<DroneShelfTile>(), 0);
            Item.width = 30;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.rare = ItemRarityID.LightPurple;
		}
    }
}