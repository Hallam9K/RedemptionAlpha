using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Items.Accessories.PreHM
{
    public class CrystallisedKnowledge : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Hitting enemies using a weapon with any element will cause elemental crystals to gradually appear, circling around the user\n" +
                "\nThe element of the crystals are based on the element of the weapon used to create them\n" +
                "Getting hit will cause the crystals to break away from the user\n" +
                "Once 6 crystals have been created, they will amass into an enchanted tome which fires bolts based on each crystal's element" +
                "\n4% increased elemental damage and resistance for each crystal of an element to be active\n\n\nThis is not done lol");
            SacrificeTotal = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.sellPrice(0, 1);
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.RedemptionPlayerBuff().crystalKnowledge = true;
        }
    }
}
