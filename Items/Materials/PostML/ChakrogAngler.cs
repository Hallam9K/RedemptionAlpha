using Redemption.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PostML
{
    public class ChakrogAngler : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 34;
            Item.value = Item.sellPrice(0, 1, 16, 0);
            Item.maxStack = Item.CommonMaxStack;
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
    }
}