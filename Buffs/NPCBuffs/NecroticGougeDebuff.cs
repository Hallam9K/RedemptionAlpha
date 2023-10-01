using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Terraria.ID;

namespace Redemption.Buffs.NPCBuffs
{
    public class NecroticGougeDebuff : ModBuff
    {
        public override string Texture => "Redemption/Buffs/Debuffs/_DebuffTemplate";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Necrotic Gouge");
            // Description.SetDefault("owie :(");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            BuffID.Sets.GrantImmunityWith[Type].Add(BuffID.Bleeding);
            BuffID.Sets.GrantImmunityWith[Type].Add(BuffID.BloodButcherer);
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.RedemptionNPCBuff().necroticGouge = true;
        }
    }
}