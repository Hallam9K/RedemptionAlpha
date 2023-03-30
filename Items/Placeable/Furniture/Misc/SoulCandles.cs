using Redemption.Rarities;
using Redemption.Tiles.Furniture.Misc;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Furniture.Misc
{
    public class SoulCandles : ModItem
	{
		public override void SetStaticDefaults()
		{
            /* Tooltip.SetDefault("Creates a small aura that instantly kills any soulless enemies that enter" +
                "\nLife regen is disabled in the aura"); */
			Item.ResearchUnlockCount = 1;
		}

		public override void SetDefaults()
		{
			Item.DefaultToPlaceableTile(ModContent.TileType<SoulCandlesTile>(), 0);
			Item.width = 28;
			Item.height = 42;
			Item.maxStack = 1;
			Item.rare = ModContent.RarityType<SoullessRarity>();
			Item.value = Item.sellPrice(0, 8, 0, 0);
        }
    }
}