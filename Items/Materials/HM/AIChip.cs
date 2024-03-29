using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
    public class AIChip : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("AI Chip");
            // Tooltip.SetDefault("'Filled with code'");
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(10, 10));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 26;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 2, 50, 0);
            Item.rare = ItemRarityID.Pink;
        }
    }
}