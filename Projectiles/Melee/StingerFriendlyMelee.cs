using Microsoft.Xna.Framework;
using Redemption.Base;
using Redemption.Projectiles.Misc;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Melee
{
    public class StingerFriendlyMelee : StingerFriendly
    {
        public override string Texture => "Redemption/Projectiles/Misc/StingerFriendly";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Stinger");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 40;
        }
    }
}