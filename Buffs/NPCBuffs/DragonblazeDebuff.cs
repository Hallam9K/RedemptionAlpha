using Terraria;
using Terraria.ModLoader;
using Redemption.Base;

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
            npc.RedemptionNPCBuff().dragonblaze = true;
        }
    }
}