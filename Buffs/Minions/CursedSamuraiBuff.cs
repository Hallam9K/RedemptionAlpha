using Redemption.BaseExtension;
using Redemption.NPCs.Friendly;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Minions
{
    public class CursedSamuraiBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            bool Active = false;
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (!npc.friendly || npc.type != ModContent.NPCType<WraithSlayer_Samurai>())
                    continue;
                Active = true;
            }
            if (Active)
                player.buffTime[buffIndex] = 18000;
            else
            {
                player.DelBuff(buffIndex);
                buffIndex--;
            }
        }
    }
}