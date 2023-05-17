using Terraria;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class VesselPickBuff : ModBuff
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Shadesurge");
			Description.SetDefault("Your mining speed is greatly increased");
			Main.buffNoTimeDisplay[Type] = false;
            Main.debuff[Type] = true;
		}
    }
}