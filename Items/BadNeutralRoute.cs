using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Items
{
    public class BadNeutralRoute : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bad Route");
            // Tooltip.SetDefault("This will decrease alignment unless you make a good decision");
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Yellow;
        }
    }
}
