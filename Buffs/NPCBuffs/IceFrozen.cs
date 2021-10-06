using Redemption.Globals.NPC;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.NPCBuffs
{
    public class IceFrozen : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Frozen");
            Description.SetDefault("brrr");
            Main.buffNoSave[Type] = true;
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            CanBeCleared = false;
        }

        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<BuffNPC>().iceFrozen = true;
        }
    }
}