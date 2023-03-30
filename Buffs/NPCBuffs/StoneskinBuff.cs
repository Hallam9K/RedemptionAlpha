using Redemption.BaseExtension;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.NPCBuffs
{
    public class StoneskinBuff : ModBuff
    {
        public override string Texture => "Redemption/Buffs/Debuffs/_DebuffTemplate";
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Stoneskin");
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.RedemptionNPCBuff().stoneskin = true;
        }
    }
}