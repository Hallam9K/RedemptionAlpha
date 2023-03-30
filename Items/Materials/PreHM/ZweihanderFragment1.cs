using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PreHM
{
    public class ZweihanderFragment1 : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rusty Zweihander Hilt");
            // Tooltip.SetDefault("'A piece of a strange weapon...'");
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<ZweihanderFragment2>();
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 42;
            Item.height = 42;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 0;
            Item.rare = ItemRarityID.Gray;
        }
    }
}
