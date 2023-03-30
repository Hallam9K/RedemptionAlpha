using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PostML
{
    public class EmptyMutagen : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Empty Mutagen");
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 36;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Purple;
        }
    }
}