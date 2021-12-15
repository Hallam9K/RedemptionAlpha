using Redemption.Globals.NPC;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.NPCBuffs
{
    public class PureChillDebuff : ModBuff
    {
        public override string Texture => "Redemption/Buffs/Debuffs/_DebuffTemplate";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pure Chill");
            Description.SetDefault("brrr");
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<BuffNPC>().pureChill = true;
        }
    }
}