using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class MoonflareCandleBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.ZoneGraveyard = false;
            if (Main.GraveyardVisualIntensity > 0)
                Main.GraveyardVisualIntensity -= 0.04f;
        }
    }
}