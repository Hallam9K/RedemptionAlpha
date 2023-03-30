using Redemption.Buffs.Debuffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Usable.Potions
{
    public class CrystalSerum : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Anti-Crystallizer Serum");
            /* Tooltip.SetDefault("Reverts xenomite infection to a previous stage\n" +
                "Cures green rashes\n"
                + "\n'Label says 'Do not swallow.' Why would you do that?'"); */
            Item.ResearchUnlockCount = 20;
        }
        public override void SetDefaults()
        {
            Item.UseSound = SoundID.Item3;
            Item.useStyle = ItemUseStyleID.DrinkLiquid;
            Item.useTurn = true;
            Item.useAnimation = 17;
            Item.useTime = 17;
            Item.maxStack = Item.CommonMaxStack;
            Item.consumable = true;
            Item.width = 12;
            Item.height = 38;
            Item.value = 100;
            Item.rare = ItemRarityID.Green;
            Item.buffType = ModContent.BuffType<SerumWithdrawalDebuff>();
            Item.buffTime = 900;
        }
        public override bool CanUseItem(Player player) => !player.HasBuff<SerumWithdrawalDebuff>();
        public override void OnConsumeItem(Player player)
        {
            if (player.HasBuff<GreenRashesDebuff>())
                player.ClearBuff(ModContent.BuffType<GreenRashesDebuff>());
            else if (player.HasBuff<GlowingPustulesDebuff>())
            {
                player.ClearBuff(ModContent.BuffType<GlowingPustulesDebuff>());
                player.AddBuff(ModContent.BuffType<GreenRashesDebuff>(), 1800);
            }
            else if (player.HasBuff<FleshCrystalsDebuff>())
            {
                player.ClearBuff(ModContent.BuffType<FleshCrystalsDebuff>());
                player.AddBuff(ModContent.BuffType<GlowingPustulesDebuff>(), 3600);
            }
        }
    }
}