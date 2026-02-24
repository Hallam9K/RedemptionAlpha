using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Buffs.NPCBuffs
{
    public class FlagbearerBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Rallied");
            // Description.SetDefault(":D");
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.RedemptionNPCBuff().rallied = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.moveSpeed *= 1.2f;
            player.statDefense += 4;
            player.GetDamage(DamageClass.Generic).Flat += 1;
        }
    }
}