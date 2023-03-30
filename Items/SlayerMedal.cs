using Redemption.Tiles.Furniture.Misc;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items
{
    public class SlayerMedal : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Medal");
            /* Tooltip.SetDefault("It reads - [c/b8eff5:'Congratulations, you beat me. Have a medal.]"
                + "\n[c/b8eff5:... Stupid dumb idiot.]"
                + "\n'It's just a piece of painted wood in the shape of a medal...'"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.DefaultToPlaceableTile(ModContent.TileType<SlayerMedalTile>());
            Item.width = 16;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 1;
            Item.rare = ItemRarityID.Blue;
        }
    }
}