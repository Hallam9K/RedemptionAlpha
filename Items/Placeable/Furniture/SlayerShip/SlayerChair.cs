using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.SlayerShip;

namespace Redemption.Items.Placeable.Furniture.SlayerShip
{
    public class SlayerChair : ModItem
	{
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Big Chair for a Big Robot");
            /* Tooltip.SetDefault("If you have this you're either cheating or you broke something" +
                "\n[c/ff0000:Unbreakable (500% Pickaxe Power after 3rd Overlord)]"); */
        }
        public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<SlayerChairTile>(), 0);
			Item.width = 26;
			Item.height = 38;
			Item.maxStack = 1;
			Item.rare = ItemRarityID.Cyan;
		}
	}
}