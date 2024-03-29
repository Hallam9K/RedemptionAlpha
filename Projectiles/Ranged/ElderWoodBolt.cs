using Microsoft.Xna.Framework;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class ElderWoodBolt : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 600;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 1;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 10;
            return true;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (targetHitbox.Width > 8 && targetHitbox.Height > 8)
                targetHitbox.Inflate(-targetHitbox.Width / 8, -targetHitbox.Height / 8);
            return projHitbox.Intersects(targetHitbox);
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Player player = Main.player[Projectile.owner];
            MakeDust();
            int rand = Main.rand.Next(2, 4);
            for (int i = 0; i < rand; i++)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity.RotatedByRandom(.4f), ModContent.ProjectileType<ElderWoodBolt_Shard>(), Projectile.damage / 2, 0, player.whoAmI);
        }
        public void MakeDust()
        {
            Vector2 positionInWorld = Main.rand.NextVector2FromRectangle(Projectile.Hitbox);
            ParticleOrchestraSettings particleOrchestraSettings = default;
            particleOrchestraSettings.PositionInWorld = positionInWorld;
            ParticleOrchestraSettings settings = particleOrchestraSettings;
            ParticleOrchestrator.RequestParticleSpawn(false, ParticleOrchestraType.SilverBulletSparkle, settings, Projectile.owner);

            for (float num4 = 0f; num4 < 3f; num4 += 2f)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 vector2 = (MathHelper.PiOver4 * num4).ToRotationVector2() * 4f;
                    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.SilverCoin, vector2.RotatedBy(Main.rand.NextFloatDirection() * (MathHelper.Pi * 2f) * 0.025f) * Main.rand.NextFloat());
                    dust.noGravity = true;
                    Dust dust2 = Dust.NewDustPerfect(Projectile.Center, DustID.SilverCoin, -vector2.RotatedBy(Main.rand.NextFloatDirection() * (MathHelper.Pi * 2f) * 0.025f) * Main.rand.NextFloat());
                    dust2.noGravity = true;
                }
            }
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Projectile.velocity.Y += 0.03f;
            if (Main.rand.NextBool(4))
            {
                int sparkle = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin, Scale: 1f);
                Main.dust[sparkle].velocity *= 0.2f;
                Main.dust[sparkle].noGravity = true;
                Main.dust[sparkle].color = new Color(255, 255, 255) { A = 0 };
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Player player = Main.player[Projectile.owner];

            for (int i = 0; i < 10; i++)
            {
                int crack = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverCoin, -Projectile.velocity.X * 0.4f, -Projectile.velocity.Y * 0.4f, Scale: 1.3f);
                Main.dust[crack].noGravity = true;
                Main.dust[crack].color = new Color(255, 255, 255) { A = 0 };
            }

            for (int i = 0; i < Main.rand.Next(2, 4); i++)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Main.rand.NextVector2Unit() * 14, ModContent.ProjectileType<ElderWoodBolt_Shard>(), Projectile.damage / 2, 0, player.whoAmI);

            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }
    }
    public class ElderWoodBolt_Shard : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + 498;
        public override void SetDefaults()
        {
            Projectile.width = 6;
            Projectile.height = 6;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.timeLeft = 10;
            Projectile.extraUpdates = 1;
            Projectile.penetrate = 2;
            Projectile.usesLocalNPCImmunity = true;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            int sparkle = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SilverFlame, Scale: 1f);
            Main.dust[sparkle].velocity *= 0.2f;
            Main.dust[sparkle].noGravity = true;
            Main.dust[sparkle].color = new Color(255, 255, 255) { A = 0 };
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 6;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            for (int i = 0; i < 10; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WhiteTorch,
                    -Projectile.velocity.X * 0.4f, -Projectile.velocity.Y * 0.4f, Scale: 1f);

            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            return true;
        }
    }
}
