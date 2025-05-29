using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.CrossMod;

namespace Redemption.Buffs.Debuffs
{
    public class DevilScentedDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;

            ThoriumHelper.AddPlayerStatusBuffID(Type);
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.stinky = true;
            player.RedemptionPlayerBuff().devilScented = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.stinky = true;
            npc.RedemptionNPCBuff().devilScented = true;
        }
    }
}