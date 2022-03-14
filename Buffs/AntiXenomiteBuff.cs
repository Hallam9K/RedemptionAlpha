using Redemption.Buffs.Debuffs;
using Terraria;
using Terraria.ModLoader;
using Redemption.Base;

namespace Redemption.Buffs
{
    public class AntiXenomiteBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Xenomite Immunity");
            Description.SetDefault("You have temporary immunity to Xenomite");
            Main.buffNoTimeDisplay[Type] = false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.RedemptionPlayerBuff().antiXenomiteBuff = true;
            player.buffImmune[ModContent.BuffType<GreenRashesDebuff>()] = true;
            player.buffImmune[ModContent.BuffType<GlowingPustulesDebuff>()] = true;
            player.buffImmune[ModContent.BuffType<FleshCrystalsDebuff>()] = true;
        }
    }
}