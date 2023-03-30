using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Items
{
    public class BadRoute : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Bad Route");
            // Tooltip.SetDefault("This will decrease alignment");
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Red;
        }
    }
}