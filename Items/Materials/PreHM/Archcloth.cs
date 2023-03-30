using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PreHM
{
    public class Archcloth : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("'Expensive, purple cloth only used by the Nobles of Anglon'");

            Item.ResearchUnlockCount = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 1, 0, 0);
            Item.rare = ItemRarityID.Orange;
        }
    }
}
