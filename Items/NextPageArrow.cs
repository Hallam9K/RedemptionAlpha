using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Items
{
    public class NextPageArrow : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Click 'Next Page' to continue dialogue");
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.rare = ItemRarityID.White;
        }
    }
}
