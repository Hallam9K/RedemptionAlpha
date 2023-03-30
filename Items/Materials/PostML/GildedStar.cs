using Redemption.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PostML
{
    public class GildedStar : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Galaxy Star");
            Item.ResearchUnlockCount = 25;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 32;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.rare = ModContent.RarityType<TurquoiseRarity>();
        }
    }
}