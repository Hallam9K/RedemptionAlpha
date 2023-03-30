using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Redemption.Tiles.Furniture.Lab;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class LargeVent : ModItem
	{
        public override void SetStaticDefaults()
        {
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<LargeVentTile>(), 0);
			Item.width = 48;
			Item.height = 32;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 100;
			Item.rare = ItemRarityID.LightPurple;
		}
	}
}