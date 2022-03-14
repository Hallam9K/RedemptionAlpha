using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Redemption.Base;

namespace Redemption.Buffs.Debuffs
{
    public class GlowingPustulesDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.RedemptionPlayerBuff().glowingPustules = true;
            player.statDefense -= 9;
            player.moveSpeed *= 0.80f;
            Lighting.AddLight(player.Center, Color.LimeGreen.ToVector3() * 0.55f);
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.lifeRegen = -20;
        }
    }
}