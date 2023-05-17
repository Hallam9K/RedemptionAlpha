using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.SlayerShip;

namespace Redemption.Items.Placeable.Furniture.SlayerShip
{
    public class KSBattlestation : ModItem
	{
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Observatory Station");
            SacrificeTotal = 1;
        }
        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<KSBattlestationTile>(), 0);
            Item.width = 60;
            Item.height = 62;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ItemRarityID.LightPurple;
		}
    }
}