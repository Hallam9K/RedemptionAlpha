using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Buffs.NPCBuffs
{
    public class RoosterBoostBuff : ModBuff
    {
        public override string Texture => "Redemption/Buffs/Debuffs/_DebuffTemplate";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rooster Boostered");
            // Description.SetDefault(":D");
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.RedemptionNPCBuff().roosterBoost = true;
        }
    }
}