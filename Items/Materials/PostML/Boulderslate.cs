using Redemption.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PostML
{
    public class Boulderslate : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
        }
        public override void SetDefaults()
        {
            Item.width = 34;
            Item.height = 34;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.maxStack = 9999;
            Item.rare = ModContent.RarityType<KingdomRarity>();
        }
    }
}