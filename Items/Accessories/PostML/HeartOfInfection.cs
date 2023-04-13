using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Terraria.DataStructures;
using Redemption.Globals;
using Terraria.Localization;

namespace Redemption.Items.Accessories.PostML
{
    public class HeartOfInfection : ModItem
    {
        public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(ElementID.PoisonS);
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Heart of Infection");
            /* Tooltip.SetDefault("The player occasionally emits a life-draining force that steals life from debuffed enemies" +
                "\n20% increased " + ElementID.PoisonS + " elemental resistance" +
                 "\n10% increased " + ElementID.PoisonS + " elemental damage"); */
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(4, 6));
            ItemID.Sets.AnimatesAsSoul[Type] = true;
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 36;
            Item.value = Item.sellPrice(0, 8, 50, 0);
            Item.rare = ItemRarityID.Expert;
            Item.expert = true;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.RedemptionPlayerBuff().infectionHeart = true;
            player.RedemptionPlayerBuff().ElementalDamage[ElementID.Poison] += 0.10f;
            player.RedemptionPlayerBuff().ElementalResistance[ElementID.Poison] += 0.20f;
        }
    }
}
