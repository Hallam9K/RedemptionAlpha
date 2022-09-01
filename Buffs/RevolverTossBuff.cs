using Redemption.BaseExtension;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Buffs
{
    public class RevolverTossBuff : ModBuff
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Cool Bonus");
            Description.SetDefault("Boosted firerate for being cool!");
        }
    }
}