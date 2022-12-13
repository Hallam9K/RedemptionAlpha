using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.SlayerShip;

namespace Redemption.Items.Placeable.Furniture.SlayerShip
{
    public class ChessTable4D : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("4D Chess Table");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<ChessTable4DTile>(), 0);
            Item.width = 44;
            Item.height = 26;
            Item.maxStack = 9999;
            Item.value = 5500;
            Item.rare = ItemRarityID.LightPurple;
		}
    }
}