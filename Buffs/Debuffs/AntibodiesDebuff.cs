using Redemption.Globals.Player;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs.Debuffs
{
    public class AntibodiesDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Xenomite Antibodies");
            Description.SetDefault("'Strong antibodies flood your blood and fight off any new Infection that may come.'");
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.GetModPlayer<BuffPlayer>().antibodiesDebuff = true;
        }
    }
}