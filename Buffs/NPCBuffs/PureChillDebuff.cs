using Redemption.Globals.NPC;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Buffs.NPCBuffs
{
    public class PureChillDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Pure Chill");
            Description.SetDefault("brrr");
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<BuffNPC>().pureChill = true;

            if (npc.HasBuff(BuffID.OnFire))
                npc.DelBuff(BuffID.OnFire);
            if (npc.HasBuff(BuffID.OnFire3))
                npc.DelBuff(BuffID.OnFire3);
            if (npc.HasBuff(ModContent.BuffType<DragonblazeDebuff>()))
                npc.DelBuff(ModContent.BuffType<DragonblazeDebuff>());
        }
    }
}