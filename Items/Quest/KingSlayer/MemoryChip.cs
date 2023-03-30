using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Quest.KingSlayer
{
    public class MemoryChip : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Memory Chip");
            // Tooltip.SetDefault("Has no use to you, but it would be a good idea to keep it for now...");

            Item.ResearchUnlockCount = 1;
        }


        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 42;
            Item.maxStack = 1;
            Item.rare = ItemRarityID.Cyan;
        }
    }
}