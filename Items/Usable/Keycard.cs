using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class Keycard : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("Unlocks Laboratory Chests and Doors"
                + "\nOnly one is needed"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 28;
            Item.rare = ItemRarityID.Cyan;
            Item.maxStack = 1;
        }
    }
}