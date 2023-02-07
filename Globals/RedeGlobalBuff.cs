using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Globals
{
    public class RedeGlobalBuff : GlobalBuff
    {
        public override void Update(int type, Terraria.NPC npc, ref int buffIndex)
        {
            switch (type)
            {
                case BuffID.OnFire:
                    if (NPCLists.Plantlike.Contains(npc.type) || NPCLists.Cold.Contains(npc.type) || NPCLists.IsSlime.Contains(npc.type))
                    {
                        npc.AddBuff(BuffID.OnFire3, npc.buffTime[buffIndex]);
                        npc.buffTime[buffIndex] -= 1;
                    }
                    break;
            }
        }
    }
}