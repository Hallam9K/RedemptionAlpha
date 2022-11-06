using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class Shadestone : ModItem
	{
        public override void SetStaticDefaults()
        {
			SacrificeTotal = 100;
		}
		public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<ShadestoneTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = 9999;
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
    }
}
