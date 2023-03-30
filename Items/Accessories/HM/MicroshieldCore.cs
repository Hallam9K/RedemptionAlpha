using Redemption.Buffs.Minions;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Accessories.HM
{
    public class MicroshieldCore : ModItem
	{
		public override void SetStaticDefaults()
		{
            /* Tooltip.SetDefault("Summons a Microshield Drone that appears whenever a hostile projectile is shot at the player\n" +
                "When a projectile hits the shield, it will release a discharge and reflect it\n" +
                "The shield can take 500 damage, once destroyed, it will take 10 seconds to reactivate"); */
            Item.ResearchUnlockCount = 1;
        }

        public override void SetDefaults()
		{
            Item.width = 18;
            Item.height = 34;
            Item.value = Item.sellPrice(0, 6, 0, 0);
            Item.expert = true;
            Item.rare = ItemRarityID.Expert;
            Item.accessory = true;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            if (hideVisual)
                return;
            player.AddBuff(ModContent.BuffType<MicroshieldDroneBuff>(), 2);
        }
    }
}
