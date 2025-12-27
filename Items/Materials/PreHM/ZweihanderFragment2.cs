using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PreHM
{
    public class ZweihanderFragment2 : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rusty Zweihander Blade");
            // Tooltip.SetDefault("'A piece of a strange weapon...'");
            ItemID.Sets.ShimmerTransformToItem[Type] = ItemType<ZweihanderFragment1>();
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 58;
            Item.height = 60;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = 0;
            Item.rare = ItemRarityID.Gray;
        }
    }
}
