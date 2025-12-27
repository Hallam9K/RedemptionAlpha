using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PreHM
{
    public class ZweihanderFragment1 : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rusty Zweihander Hilt");
            // Tooltip.SetDefault("'A piece of a strange weapon...'");
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<ZweihanderFragment2>();
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 38;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 0;
            Item.rare = ItemRarityID.Gray;
        }
    }
}
