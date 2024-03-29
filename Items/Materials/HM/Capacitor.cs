using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
    public class Capacitor : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Capacitor");
            // Tooltip.SetDefault("'Holds a high amount of energy'");
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 30;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 1);
            Item.rare = ItemRarityID.Pink;
        }
    }
}