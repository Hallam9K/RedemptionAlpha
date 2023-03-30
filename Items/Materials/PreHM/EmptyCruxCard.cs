using Terraria.ID;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Items.Materials.PreHM
{
    public class EmptyCruxCard : ModItem
    {
        public override void SetStaticDefaults()
        {
            /* Tooltip.SetDefault("A container for friendly spirits to aid you\n" +
                "Find willing spirits and request their crux to imbue into the card" +
                "\n'Those who peek into the realm of fulfilment are bound to find friends'"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 40;
            Item.maxStack = 1;
            Item.value = Item.sellPrice(0, 0, 50, 0);
            Item.rare = ItemRarityID.Blue;
        }
    }
}
