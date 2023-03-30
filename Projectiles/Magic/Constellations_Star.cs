using Microsoft.Xna.Framework;
using ParticleLibrary;
using Redemption.Globals;
using Redemption.Particles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class Constellations_Star : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Constellations");
            Main.projFrames[Projectile.type] = 7;
            ElementID.ProjCelestial[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 30;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 180;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 7;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;
        public override bool? CanDamage() => Projectile.localAI[0] >= 5 && Projectile.localAI[0] < 50;
        private Vector2 pos;
        public override void OnSpawn(IEntitySource source)
        {
            if (Projectile.ai[1] == 0 && Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < Main.rand.Next(2, 7); i++)
                {
                    pos = RedeHelper.RotateVector(Main.MouseWorld, Projectile.Center, MathHelper.Pi);
                    if (i > 0)
                    {
                        Vector2 pos2 = Projectile.Center + RedeHelper.PolarVector(Main.rand.Next(100, 300), RedeHelper.RandomRotation());
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos2, Projectile.velocity, Type, Projectile.damage, Projectile.knockBack, Main.myPlayer, 0, i + 1);
                    }
                    else
                        Projectile.NewProjectile(Projectile.GetSource_FromAI(), pos, Projectile.velocity, Type, Projectile.damage, Projectile.knockBack, Main.myPlayer, 0, i + 1);
                }
            }
        }
        Projectile closeProj;
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 7)
                    Projectile.frame = 2;
            }
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.4f, Projectile.Opacity * 0.4f, Projectile.Opacity * 0.4f);
            if (Projectile.localAI[0]++ == 5)
            {
                if (Projectile.ai[1] > 0)
                {
                    if (ClosestProj(ref closeProj, 2000, Projectile.Center, true, -1, Type))
                    {
                        int steps = (int)Projectile.Distance(closeProj.Center) / 3;
                        for (int i = 0; i < steps; i++)
                            ParticleManager.NewParticle(Vector2.Lerp(Projectile.Center, closeProj.Center, (float)i / steps), Vector2.Zero, new GlowParticle2(), Color.White, 0.14f, .45f, Main.rand.Next(50, 60));
                    }
                }
                else
                {
                    int steps = (int)Projectile.Distance(pos) / 3;
                    for (int i = 0; i < steps; i++)
                        ParticleManager.NewParticle(Vector2.Lerp(Projectile.Center, pos, (float)i / steps), Vector2.Zero, new GlowParticle2(), Color.White, 0.14f, .45f, Main.rand.Next(50, 60));
                }
                Projectile.ai[0] = 1;
            }
            if (Projectile.localAI[0] >= 60)
            {
                Projectile.alpha += 5;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
            }
        }
        private bool ClosestProj(ref Projectile target, float maxDistance, Vector2 position,
    bool ignoreTiles = false, int overrideTarget = -1, int type = -1)
        {
            bool foundTarget = false;
            if (overrideTarget != -1 && (Main.npc[overrideTarget].Center - position).Length() < maxDistance)
            {
                target = Main.projectile[overrideTarget];
                return true;
            }
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                float distance = (proj.Center - position).Length();
                if (!(distance < maxDistance) || !proj.active || type != 1 && proj.type != type || proj.whoAmI == Projectile.whoAmI || proj.ai[0] != 0 || !Collision.CanHit(position, 0, 0, proj.Center, 0, 0) && !ignoreTiles)
                    continue;

                target = proj;
                foundTarget = true;
                maxDistance = (target.Center - position).Length();
            }

            return foundTarget;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            Vector2 end = pos;
            if (Projectile.ai[1] > 0 && closeProj != null)
                end = closeProj.Center;
            float point = 0f;
            // Run an AABB versus Line check to look for collisions
            if (end != Vector2.Zero && Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center,
                end, 10, ref point))
                return true;
            return false;
        }
    }
}