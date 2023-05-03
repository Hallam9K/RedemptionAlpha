using Redemption.Rarities;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PostML
{
    public class AbyssStinger : ModItem
    {
        public override void SetStaticDefaults()
        {
            SacrificeTotal = 3;
        }

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 30;
            Item.value = Item.sellPrice(0, 1, 0, 0);
            Item.maxStack = 9999;
            Item.rare = ModContent.RarityType<SoullessRarity>();
        }
    }
}