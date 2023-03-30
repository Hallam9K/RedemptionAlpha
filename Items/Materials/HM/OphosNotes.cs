using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
    public class OphosNotes : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Ophos' Lost Notes");
            // Tooltip.SetDefault("'Notes discovered by the Wayfarer detailing the smithing process of a famous sword'");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 36;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 45, 0, 0);
            Item.rare = ItemRarityID.Gray;
        }
    }
}