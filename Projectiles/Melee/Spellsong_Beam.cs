using Microsoft.Xna.Framework;
using ParticleLibrary;
using Redemption.BaseExtension;
using Redemption.Globals;
using Redemption.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Melee
{
    public class Spellsong_Beam : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Arcane Beam");
        }

        public override void SetDefaults()
        {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.extraUpdates = 100;
            Projectile.timeLeft = 350;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Melee;
        }
        public override void AI()
        {
            if (Projectile.localAI[0]++ > 90)
            {
                for (int i = 0; i < 1; i++)
                {
                    Vector2 v = Projectile.position;
                    v -= Projectile.velocity * (i * 0.25f);
                    ParticleManager.NewParticle(v, Vector2.Zero, new GlowParticle2(), Color.HotPink, Main.rand.NextFloat(0.3f, 0.4f), .45f, Main.rand.Next(10, 20));
                }
            }
        }
        public override void OnKill(int timeLeft)
        {
            Player player = Main.player[Projectile.owner];
            RedeDraw.SpawnRing(Projectile.Center, Color.White, glowScale: 8);
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.NebSound3 with { Volume = 0.2f, Pitch = 0.1f }, Projectile.position);
            Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
            Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 7;

            for (int i = 0; i < Main.rand.Next(3, 5); i++)
            {
                if (Projectile.owner == Main.myPlayer)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.Spread(10), ModContent.ProjectileType<SpellsongMirage_Proj>(), Projectile.damage, Projectile.knockBack, player.whoAmI);
            }
        }
    }
}
