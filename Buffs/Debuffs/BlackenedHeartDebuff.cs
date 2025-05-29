using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.CrossMod;

namespace Redemption.Buffs.Debuffs
{
    public class BlackenedHeartDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = false;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.LongerExpertDebuff[Type] = true;

            ThoriumHelper.AddPlayerDoTBuffID(Type);
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.statLifeMax2 -= 100;
            player.lifeRegen -= 400;
            player.blind = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.RedemptionNPCBuff().blackHeart = true;
        }
    }
}
