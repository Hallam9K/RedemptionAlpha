using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

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
            Item.width = 26;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 0, 10, 0);
            Item.rare = ItemRarityID.Pink;
        }
    }
}