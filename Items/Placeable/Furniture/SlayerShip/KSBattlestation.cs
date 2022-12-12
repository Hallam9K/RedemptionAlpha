using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.SlayerShip;
using Terraria.GameContent.Creative;
using Redemption.Items.Placeable.Tiles;
using Redemption.Items.Materials.HM;

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
            Item.maxStack = 99;
            Item.rare = ItemRarityID.LightPurple;
		}
    }
}