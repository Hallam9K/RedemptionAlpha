using Microsoft.Xna.Framework;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class BlueOrb : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Blue Orb");
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 240;
            Projectile.GetGlobalProjectile<RedeGlobalProjectile>().Unparryable = true;
        }

        public override void AI()
        {
            
        }

        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GemSapphire, 0, 0, Scale: 1);
                Main.dust[dust].velocity *= 2f;
                Main.dust[dust].noGravity = true;
            }
        }           
    }
}