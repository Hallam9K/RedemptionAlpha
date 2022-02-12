using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Redemption.BaseExtension;

namespace Redemption.Buffs.Debuffs
{
    public class FleshCrystalsDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = false;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.RedemptionPlayerBuff().fleshCrystals = true;
            player.statDefense -= 11;
            player.moveSpeed *= 0.70f;
            player.AddBuff(ModContent.BuffType<HemorrhageDebuff>(), 1800);
            player.AddBuff(ModContent.BuffType<NecrosisDebuff>(), 3600);
            Lighting.AddLight(player.Center, Color.LimeGreen.ToVector3() * 0.7f);
        }
    }
}