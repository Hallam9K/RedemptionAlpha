using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
    public class Capacitator : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Capacitator");
            Tooltip.SetDefault("'Holds an extreme amount of energy'");
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 30;
            Item.maxStack = 30;
            Item.value = 650000;
            Item.rare = ItemRarityID.Lime;
        }
    }
}