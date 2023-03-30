using Redemption.Tiles.Containers;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Containers
{
    public class Holochest : ModItem
	{
		public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Holochest");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(5, 2));
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<HolochestTile>(), 0);
			Item.width = 32;
			Item.height = 28;
			Item.maxStack = Item.CommonMaxStack;
			Item.value = 500;
		}
	}
}