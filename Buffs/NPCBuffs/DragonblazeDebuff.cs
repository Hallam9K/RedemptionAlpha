using Redemption.Buffs.Debuffs;
using Redemption.Globals.NPC;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Buffs.NPCBuffs
{
    public class DragonblazeDebuff : ModBuff
    {
        public override string Texture => "Redemption/Buffs/Debuffs/_DebuffTemplate";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Dragonblaze");
            Description.SetDefault("grrr");
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<BuffNPC>().dragonblaze = true;
        }
    }
}