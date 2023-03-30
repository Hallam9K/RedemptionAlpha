using Terraria.ID;
using Terraria.ModLoader;
using Terraria;

namespace Redemption.Items.Materials.HM
{
    public class CarbonMyofibre : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Carbon Myofibre");
            // Tooltip.SetDefault("'Elastic and strong'");
            Item.ResearchUnlockCount = 50;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 5000;
            Item.rare = ItemRarityID.Pink;
        }
    }
}