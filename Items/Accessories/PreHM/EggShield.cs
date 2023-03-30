using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    [AutoloadEquip(EquipType.Shield)]
    public class EggShield : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Egg Shelld");
            /* Tooltip.SetDefault("Decreases knockback the user receives\n" +
                "When below 25% health, you will completely ignore knockback"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 22;
            Item.height = 28;
            Item.value = Item.buyPrice(0, 0, 20, 0);
            Item.hasVanityEffects = true;
            Item.rare = ItemRarityID.Blue;
            Item.accessory = true;
            Item.defense = 2;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (player.statLife <= player.statLifeMax2 / 4)
                player.noKnockback = true;
        }
    }
}
