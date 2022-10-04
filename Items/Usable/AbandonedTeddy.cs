using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class AbandonedTeddy : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Let the Keeper's spirit rest" +
                "\n[i:" + ModContent.ItemType<RedemptionRoute>() + "]");

            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 36;
            Item.height = 48;
            Item.maxStack = 1;
            Item.rare = -1;
        }
    }
}
