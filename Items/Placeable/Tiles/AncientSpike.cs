using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class AncientSpike : ModItem
	{
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Ancient Spike");
            //Tooltip.SetDefault("[c/ff0000:Unbreakable (1000% Pickaxe Power)]");
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<AncientSpikeTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
    }
}
