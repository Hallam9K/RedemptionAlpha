using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PreHM
{
    public class TreeBugShell : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tree Bug Shell");
            ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<CoastScarabShell>();
            Item.ResearchUnlockCount = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 14;
            Item.height = 22;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(copper: 50);
            Item.rare = ItemRarityID.White;
        }
    }
}