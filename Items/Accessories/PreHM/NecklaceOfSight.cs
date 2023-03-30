using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.PreHM
{
    [AutoloadEquip(EquipType.Neck)]
    public class NecklaceOfSight : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Necklace of Sight");
            /* Tooltip.SetDefault("Increases movement speed while an enemy is close\n" +
                "6% increased critical strike chance\n"
                + "Improves vision"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 26;
            Item.height = 30;
            Item.value = Item.sellPrice(0, 1, 50, 0);
            Item.rare = ItemRarityID.Expert;
            Item.hasVanityEffects = true;
            Item.accessory = true;
            Item.expert = true;
        }
        NPC target;
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.nightVision = true;
            player.GetCritChance<GenericDamageClass>() += 6;
            if (RedeHelper.ClosestNPC(ref target, 200, player.Center, true))
                player.AddBuff(BuffID.Panic, 60);
        }
    }
}
