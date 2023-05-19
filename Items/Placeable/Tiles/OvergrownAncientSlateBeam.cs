using Redemption.Rarities;
using Redemption.Tiles.Tiles;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Placeable.Tiles
{
    public class OvergrownAncientSlateBeam : ModItem
	{
        public override void SetStaticDefaults()
        {
            //DisplayName.SetDefault("Overgrown Ancient Slate Beam");
            //Tooltip.SetDefault("[c/ff0000:Unbreakable]");
            Item.ResearchUnlockCount = 100;
        }

        public override void SetDefaults()
		{
            Item.DefaultToPlaceableTile(ModContent.TileType<OvergrownAncientSlateBeamTile>(), 0);
            Item.width = 16;
            Item.height = 16;
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
    }
}
