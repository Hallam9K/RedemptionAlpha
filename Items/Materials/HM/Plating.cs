using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
    public class Plating : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Plating");
            Tooltip.SetDefault("Resistant to extreme impacts...");
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = 30;
            Item.value = 650000;
            Item.rare = ItemRarityID.Lime;
        }
    }
}