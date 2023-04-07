using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Usable
{
    public class AbandonedTeddy : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Let the Keeper's spirit rest");

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
