using Microsoft.Xna.Framework;
using Redemption.NPCs.Critters;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class InfestedDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Infested");
            Description.SetDefault("Larva is eating away at your flesh");
            Main.debuff[Type] = true;
            Main.pvpBuff[Type] = true;
            Main.buffNoSave[Type] = true;
            LongerExpertDebuff = true;
        }
        public override bool ReApply(Player player, int time, int buffIndex)
        {
            player.GetModPlayer<BuffPlayer>().infestedTime += 60;
            return false;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<BuffPlayer>().infested = true;
        }
        public override bool ReApply(NPC npc, int time, int buffIndex)
        {
            npc.GetGlobalNPC<BuffNPC>().infestedTime += 60;
            return false;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.GetGlobalNPC<BuffNPC>().infested = true;
        }
    }
}