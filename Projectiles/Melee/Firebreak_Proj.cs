using Microsoft.Xna.Framework;
using Redemption.Dusts;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using ParticleLibrary;
using Redemption.Particles;
using Redemption.Projectiles.Magic;

namespace Redemption.Projectiles.Melee
{
    public class Firebreak_Proj : CantripEmber
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Flaming Rain");
            ElementID.ProjFire[Type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 2;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 200;
            Projectile.usesLocalNPCImmunity = true;
        }

        public override void OnKill(int timeLeft)
        {
            if (fakeTimer > 0)
                return;

            if (!Projectile.wet)
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode with { Volume = .4f }, Projectile.position);
            for (int i = 0; i < 6; i++)
                ParticleManager.NewParticle(Projectile.Center, RedeHelper.SpreadUp(1), new EmberParticle(), Color.White, 1);
            for (int i = 0; i < 12; i++)
            {
                int dust = Dust.NewDust(Projectile.Center + Projectile.velocity, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.2f);
                Main.dust[dust].noGravity = true;
                Color dustColor = new(253, 221, 3) { A = 0 };
                Main.dust[dust].color = dustColor;
                int dust2 = Dust.NewDust(Projectile.Center + Projectile.velocity, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, 0.2f);
                Main.dust[dust2].noGravity = true;
                Color dustColor2 = new(253, 62, 3) { A = 0 };
                Main.dust[dust2].color = dustColor2;
            }
            for (int i = 0; i < 24; i++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Torch, 0, 0, Scale: 2);
                Main.dust[dust].velocity *= 10f;
                Main.dust[dust].noGravity = true;
            }
        }
    }
}