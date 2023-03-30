using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
    public class Plating : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Plating");
            // Tooltip.SetDefault("Resistant to extreme impacts");
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 65000;
            Item.rare = ItemRarityID.Pink;
        }
    }
}