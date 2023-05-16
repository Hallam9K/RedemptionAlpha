using Terraria.ModLoader;
using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Redemption.Tiles.Plants;

namespace Redemption.Items.Placeable.Plants
{
    public class Nooseroot : ModItem
	{
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 25;
        }
        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<NooserootVines>(), 0);
            Item.maxStack = 9999;
            Item.width = 18;
            Item.height = 32;
            Item.value = 200;
            Item.rare = ModContent.RarityType<SoullessRarity>();
		}
	}
}