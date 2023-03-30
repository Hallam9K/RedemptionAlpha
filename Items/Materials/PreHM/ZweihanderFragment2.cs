using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PreHM
{
    public class ZweihanderFragment2 : ModItem
    {
        public override void SetStaticDefaults()
		{
            // DisplayName.SetDefault("Rusty Zweihander Blade");
            // Tooltip.SetDefault("'A piece of a strange weapon...'");
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<ZweihanderFragment1>();
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
