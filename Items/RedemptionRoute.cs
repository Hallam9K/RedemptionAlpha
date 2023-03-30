using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Items
{
    public class RedemptionRoute : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Redemption");
            // Tooltip.SetDefault("This will redeem your past actions");
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Yellow;
        }
    }
}
