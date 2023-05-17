using Terraria.ModLoader;
using Terraria;
using Redemption.Rarities;
using Redemption.Tiles.Plants;

namespace Redemption.Items.Placeable.Plants
{
    public class Nooseroot : ModItem
	{
        public override void SetStaticDefaults()
        {
            Item.ResearchUnlockCount = 25;
        }
        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<NooserootVines>(), 0);
            Item.maxStack = Item.CommonMaxStack;
            Item.width = 18;
            Item.height = 32;
            Item.value = 200;
            Item.rare = ModContent.RarityType<SoullessRarity>();
		}
	}
}