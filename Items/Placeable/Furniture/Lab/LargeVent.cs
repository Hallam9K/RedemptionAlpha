using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Tiles.Furniture.Lab;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LargeVent : ModItem
	{
        public override void SetStaticDefaults()
        {
			SacrificeTotal = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<LargeVentTile>(), 0);
			Item.width = 48;
			Item.height = 32;
			Item.maxStack = 9999;
			Item.value = 100;
			Item.rare = ItemRarityID.LightPurple;
		}
	}
}