using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria.ModLoader;
using Terraria;

namespace Redemption.Items.Placeable.Tiles
{
    public class Shadestone : ModItem
	{
        public override void SetStaticDefaults()
        {
			Item.ResearchUnlockCount = 100;
		}
		public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<ShadestoneTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
    }
}
