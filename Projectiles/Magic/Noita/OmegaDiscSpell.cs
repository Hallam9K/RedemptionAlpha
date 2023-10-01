using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic.Noita
{
    public class OmegaDiscSpell : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Omega Disc");
        }
        public override void SetDefaults()
        {
            Projectile.width = 46;
            Projectile.height = 46;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 800;
        }
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (Projectile.localAI[1]++ >= 10)
            {
                if (Collision.SolidCollision(Projectile.position - new Vector2(8, 8), Projectile.width + 16, Projectile.height + 16))
                {
                    if (Projectile.localAI[1] % 10 == 0)
                    {
                        Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                        SoundEngine.PlaySound(SoundID.Item23 with { Volume = .2f, Pitch = -.5f }, Projectile.position);
                        for (int i = 0; i < 6; i++)
                        {
                            int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke,
                                -Projectile.velocity.X * 0.4f, -Projectile.velocity.Y * 0.4f);
                            Main.dust[d].noGravity = true;
                        }
                        for (int i = 0; i < 10; i++)
                        {
                            int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.MartianSaucerSpark,
                                -Projectile.velocity.X * 2f, -Projectile.velocity.Y * 2f, Scale: 0.5f);
                            Main.dust[d].noGravity = true;
                        }
                    }
                    Projectile.velocity *= .98f;
                }
                Projectile.Move(player.Center, 6, 40);
                Projectile.tileCollide = false;
                if (Projectile.Hitbox.Intersects(player.Hitbox))
                    Projectile.Kill();
            }
            Projectile.rotation += .4f * Projectile.direction;
            if (Projectile.localAI[0]++ % 6 == 0)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<DiscAfterimage>(), 0, 0, Projectile.owner, Projectile.rotation);
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.MartianSaucerSpark,
                    Projectile.velocity.X, Projectile.velocity.Y, Scale: 2);
                Main.dust[d].noGravity = true;
            }
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.MartianSaucerSpark, -Projectile.velocity.X * 0.5f,
                    -Projectile.velocity.Y * 0.5f);
                Main.dust[dust].noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.Item23 with { Pitch = -.7f}, Projectile.position);
        }
    }
}