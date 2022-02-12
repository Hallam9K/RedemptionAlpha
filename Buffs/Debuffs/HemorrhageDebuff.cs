using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Redemption.BaseExtension;

namespace Redemption.Buffs.Debuffs
{
    public class HemorrhageDebuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            Main.debuff[Type] = true;
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.RedemptionPlayerBuff().hemorrhageDebuff = true;
            if (player.velocity != Vector2.Zero)
            {
                player.lifeRegen -= 25;
            }
        }
    }
}