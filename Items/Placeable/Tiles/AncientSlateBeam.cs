using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class AncientSlateBeam : ModItem
	{
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Ancient Slate Beam");
            //Tooltip.SetDefault("[c/ff0000:Unbreakable]");
            Item.ResearchUnlockCount = 200;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<AncientSlateBeamTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
    }
}
