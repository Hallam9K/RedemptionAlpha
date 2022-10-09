using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Terraria.GameContent.Creative;

namespace Redemption.Items.Materials.PreHM
{
    public class Archcloth : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("'Expensive, purple cloth only used by the Nobles of Anglon'");

            SacrificeTotal = 5;
        }

        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 26;
            Item.maxStack = 9999;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Orange;
        }
    }
}
