using Terraria.ModLoader;
using Terraria;
using Terraria.ID;

namespace Redemption.Items.Placeable.Plants
{
    public class Nightshade : ModItem
    {
        public override void SetStaticDefaults()
        {
            // Tooltip.SetDefault("'A purple plant that blooms in the night'");
            Item.ResearchUnlockCount = 25;
        }
        public override void SetDefaults()
        {
            Item.maxStack = Item.CommonMaxStack;
            Item.width = 16;
            Item.height = 20;
            Item.value = 150;
            Item.rare = ItemRarityID.Blue;
        }
    }
}
