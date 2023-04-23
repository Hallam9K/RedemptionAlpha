using Terraria;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.Buffs.Debuffs
{
    public class PureChillDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Pure Chill");
            // Description.SetDefault("Reduced movement and loss of life");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }
        public override void Update(NPC npc, ref int buffIndex)
        {
            npc.RedemptionNPCBuff().pureChill = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            if (player.velocity.Y < 0)
                player.velocity.Y *= 0.94f;
            player.velocity.X *= 0.94f;
            player.RedemptionPlayerBuff().pureChill = true;
        }
    }
}
