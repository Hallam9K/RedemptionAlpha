using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.HM
{
    public class CyberPlating : ModItem
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cyber Plating");
            Tooltip.SetDefault("'Resistant to everything'");
            SacrificeTotal = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.maxStack = 9999;
            Item.value = Item.buyPrice(0, 5, 0, 0);
            Item.rare = ItemRarityID.Cyan;
        }
    }
}