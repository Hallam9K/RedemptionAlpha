using Terraria.ModLoader;
using Terraria.ID;
using Terraria;
using Redemption.Tiles.Furniture.Lab;

namespace Redemption.Items.Placeable.Furniture.Lab
{
    public class Vent : ModItem
	{
        public override void SetStaticDefaults()
        {
			Item.ResearchUnlockCount = 1;
		}
		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<VentTile>(), 0);
			Item.width = 32;
			Item.height = 24;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 100;
			Item.rare = ItemRarityID.LightPurple;
		}
	}
}