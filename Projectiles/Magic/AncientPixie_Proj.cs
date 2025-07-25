using ParticleLibrary.Utilities;
using Redemption.Globals;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class AncientPixie_Proj : ModProjectile
    {
        private Player Owner => Main.player[Projectile.owner];
        private Vector2 originPos;
        private float direction;

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            ElementID.ProjArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 600;
            Projectile.penetrate = 2;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.ai[1] == 1 && Main.rand.NextBool(3))
                target.AddBuff(BuffID.Poisoned, 180);
        }

        bool onSpawn;
        public override void AI()
        {
            int dustType = Projectile.ai[1] == 1 ? DustID.VenomStaff : DustID.DryadsWard;
            if (!onSpawn)
            {
                for (int k = 0; k < 18; k++)
                {
                    int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, Scale: 1.5f);
                    Main.dust[dust].noGravity = true;
                }

                if (Projectile.ai[1] == 1)
                    Projectile.GetGlobalProjectile<ElementalProjectile>().OverrideElement[ElementID.Poison] = 1;
                else
                    Projectile.GetGlobalProjectile<ElementalProjectile>().OverrideElement[ElementID.Nature] = 1;

                onSpawn = true;
                Projectile.netUpdate = true;
            }

            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                {
                    Projectile.frame = 0;
                }
            }
            if (Projectile.ai[1] == 1)
                Lighting.AddLight(Projectile.Center, 0.2f * Projectile.Opacity, .1f * Projectile.Opacity, 0.5f * Projectile.Opacity);
            else
                Lighting.AddLight(Projectile.Center, 0.2f * Projectile.Opacity, .5f * Projectile.Opacity, 0.1f * Projectile.Opacity);

            if (Main.rand.NextBool(20))
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, dustType);
                Main.dust[dust].velocity *= 0;
                Main.dust[dust].noGravity = true;
            }

            if (Owner.dead || !Owner.active)
            {
                Projectile.Kill();
                return;
            }

            switch (Projectile.ai[2])
            {
                default:
                    Projectile.spriteDirection = Owner.direction;
                    Projectile.direction = Owner.direction;
                    Projectile.localAI[0] += 0.08f;
                    Projectile.Center = Owner.Center + Vector2.One.RotatedBy(MathHelper.ToRadians(Projectile.ai[0] * 45) + Projectile.localAI[0]) * 50;
                    break;
                case 1:
                    Projectile.LookByVelocity();

                    for (int k = 0; k < 12; k++)
                    {
                        int dust = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, dustType, Scale: 1.5f);
                        Main.dust[dust].noGravity = true;
                    }

                    originPos = Owner.Center;
                    direction = (Main.MouseWorld - Owner.Center).ToRotation();
                    Projectile.localAI[0] += 0.08f;
                    Projectile.Center = Owner.Center + Vector2.One.RotatedBy(MathHelper.ToRadians(Projectile.ai[0] * 45) + Projectile.localAI[0]) * 50;
                    Projectile.timeLeft = 60;

                    Projectile.ai[2]++;
                    break;
                case 2:
                    Projectile.LookByVelocity();

                    Projectile.localAI[0] += 0.08f;
                    Projectile.Center = originPos + Vector2.One.RotatedBy(MathHelper.ToRadians(Projectile.ai[0] * 45) + Projectile.localAI[0]) * 50;
                    originPos += RedeHelper.PolarVector(10, direction);

                    if (Collision.SolidCollision(originPos - new Vector2(8), 16, 16))
                        Projectile.Kill();
                    break;
            }
        }
        public override void OnKill(int timeLeft)
        {
            int dustType = Projectile.ai[1] == 1 ? DustID.VenomStaff : DustID.DryadsWard;
            for (int i = 0; i < 30; i++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, dustType, Scale: 1.5f);
                Main.dust[dust].noGravity = true;
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.ai[1] == 1)
                return new Color(255, 188, 252).WithAlpha(0.5f) * Projectile.Opacity;
            return new Color(188, 255, 148).WithAlpha(0.5f) * Projectile.Opacity;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (Projectile.ai[2] == 0)
                modifiers.HitDirectionOverride = target.RightOfDir(Owner);
            else
                modifiers.HitDirectionOverride = Projectile.spriteDirection;
        }
    }
}