using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace Redemption.Items
{
    public class GoodRoute : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Good Route");
            // Tooltip.SetDefault("This will increase alignment");
        }

        public override void SetDefaults()
        {
            Item.width = 16;
            Item.height = 16;
            Item.rare = ItemRarityID.Lime;
        }
    }
}
