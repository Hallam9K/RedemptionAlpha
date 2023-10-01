using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.Minions;
using Redemption.Buffs.NPCBuffs;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Redemption.Projectiles.Minions
{
    public class TeslaGenerator_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tesla Field Generator");

            ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 42;
            Projectile.tileCollide = true;
            Projectile.sentry = true;
            Projectile.timeLeft = Projectile.SentryLifeTime;

            Projectile.friendly = true;
            Projectile.ignoreWater = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
        }

        public override bool? CanDamage() => false;
        private NPC target;
        public override void AI()
        {
            Player owner = Main.player[Projectile.owner];

            if (!CheckActive(owner))
                return;

            Projectile.velocity.Y += 1;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC target = Main.npc[i];
                if (!target.active || !target.CanBeChasedBy())
                    continue;

                if (Projectile.DistanceSQ(target.Center) > 400 * 400)
                    continue;

                target.AddBuff(ModContent.BuffType<ElectrifiedDebuff>(), 10);
            }
            if (RedeHelper.ClosestNPC(ref target, 400, Projectile.Center, false, owner.MinionAttackTargetNPC))
            {
                if (Projectile.localAI[0]++ % 50 == 0 && Main.myPlayer == Projectile.owner)
                {
                    Vector2 ai = RedeHelper.PolarVector(10, (target.Center - Projectile.Center).ToRotation());
                    float ai2 = Main.rand.Next(100);
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.PolarVector(10, (target.Center - Projectile.Center).ToRotation()), ModContent.ProjectileType<TeslaGenerator_Lightning>(), Projectile.damage, 0, Main.myPlayer, ai.ToRotation(), ai2);
                }
            }
            for (int k = 0; k < 2; k++)
            {
                Vector2 vector;
                double angle = Main.rand.NextDouble() * 2d * Math.PI;
                vector.X = (float)(Math.Sin(angle) * 400);
                vector.Y = (float)(Math.Cos(angle) * 400);
                Dust dust2 = Main.dust[Dust.NewDust(Projectile.Center + vector, 2, 2, DustID.Electric)];
                dust2.noGravity = true;
                dust2.velocity = -Projectile.DirectionTo(dust2.position) * 4f;
            }
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            fallThrough = false;
            return true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (oldVelocity.Y > 0)
                Projectile.velocity.Y = 0;
            return false;
        }
        private bool CheckActive(Player owner)
        {
            if (owner.dead || !owner.active)
            {
                owner.ClearBuff(ModContent.BuffType<TeslaGeneratorBuff>());
                return false;
            }

            if (owner.HasBuff(ModContent.BuffType<TeslaGeneratorBuff>()))
                Projectile.timeLeft = 2;

            return true;
        }
    }
    public class TeslaGenerator_Lightning : ModProjectile
    {
        public override string Texture => "Terraria/Images/Projectile_" + ProjectileID.CultistBossLightningOrbArc;

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lightning Arc");
            ProjectileID.Sets.MinionShot[Projectile.type] = true;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
            ElementID.ProjThunder[Type] = true;
        }

        float colorlerp;
        bool playedsound = false;
        public override void SetDefaults()
        {
            Projectile.width = 20;
            Projectile.height = 20;
            Projectile.scale = 0.5f;
            Projectile.aiStyle = -1;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.alpha = 100;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.extraUpdates = 3;
            Projectile.timeLeft = 30 * (Projectile.extraUpdates + 1);
            Projectile.penetrate = -1;

            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 10;
        }

        public override void AI()
        {
            Projectile.frameCounter = Projectile.frameCounter + 1;
            Lighting.AddLight(Projectile.Center, 0.3f, 0.45f, 0.5f);
            colorlerp += 0.05f;

            if (!playedsound)
            {
                SoundEngine.PlaySound(SoundID.Item122 with { Volume = .5f, Pitch = -.5f }, Projectile.Center);
                playedsound = true;
            }

            if (Projectile.velocity == Vector2.Zero)
            {
                if (Projectile.frameCounter >= Projectile.extraUpdates * 2)
                {
                    Projectile.frameCounter = 0;
                    bool flag = true;
                    for (int index = 1; index < Projectile.oldPos.Length; ++index)
                    {
                        if (Projectile.oldPos[index] != Projectile.oldPos[0])
                            flag = false;
                    }
                    if (flag)
                    {
                        Projectile.Kill();
                        return;
                    }
                }
                if (!Main.rand.NextBool(Projectile.extraUpdates))
                    return;
                for (int index1 = 0; index1 < 2; ++index1)
                {
                    float num1 = Projectile.rotation + (float)((Main.rand.NextBool(2)? -1.0 : 1.0) * 1.57079637050629);
                    float num2 = (float)(Main.rand.NextDouble() * 0.800000011920929 + 1.0);
                    Vector2 vector2 = new((float)Math.Cos(num1) * num2, (float)Math.Sin(num1) * num2);
                    int index2 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Electric, vector2.X, vector2.Y, 0, new Color(), 1f);
                    Main.dust[index2].noGravity = true;
                    Main.dust[index2].scale = 1.2f;
                }
                if (!Main.rand.NextBool(5))
                    return;
                int index3 = Dust.NewDust(Projectile.Center + Projectile.velocity.RotatedBy(1.57079637050629, new Vector2()) * ((float)Main.rand.NextDouble() - 0.5f) * Projectile.width - Vector2.One * 4f, 8, 8, DustID.Smoke, 0.0f, 0.0f, 100, new Color(), 1.5f);
                Dust dust = Main.dust[index3];
                dust.velocity *= 0.5f;
                Main.dust[index3].velocity.Y = -Math.Abs(Main.dust[index3].velocity.Y);
            }
            else
            {
                if (Projectile.frameCounter < Projectile.extraUpdates * 2)
                    return;
                Projectile.frameCounter = 0;
                float num1 = Projectile.velocity.Length();
                UnifiedRandom unifiedRandom = new((int)Projectile.ai[1]);
                int num2 = 0;
                Vector2 spinningpoint = -Vector2.UnitY;
                Vector2 rotationVector2;
                int num3;
                do
                {
                    int num4 = unifiedRandom.Next();
                    Projectile.ai[1] = num4;
                    rotationVector2 = ((float)(num4 % 100 / 100.0 * 6.28318548202515)).ToRotationVector2();
                    if (rotationVector2.Y > 0.0)
                        rotationVector2.Y--;
                    bool flag = false;
                    if (rotationVector2.Y > -0.0199999995529652)
                        flag = true;
                    if (rotationVector2.X * (double)(Projectile.extraUpdates + 1) * 2.0 * num1 + Projectile.localAI[0] > 40.0)
                        flag = true;
                    if (rotationVector2.X * (double)(Projectile.extraUpdates + 1) * 2.0 * num1 + Projectile.localAI[0] < -40.0)
                        flag = true;
                    if (flag)
                    {
                        num3 = num2;
                        num2 = num3 + 1;
                    }
                    else
                        goto label_3460;
                }
                while (num3 < 100);
                Projectile.velocity = Vector2.Zero;
                Projectile.localAI[1] = 1f;
                goto label_3461;
            label_3460:
                spinningpoint = rotationVector2;
            label_3461:
                if (Projectile.velocity == Vector2.Zero || Projectile.velocity.Length() < 4f)
                {
                    Projectile.velocity = Vector2.UnitX.RotatedBy(Projectile.ai[0]).RotatedByRandom(Math.PI / 4) * 7f;
                    Projectile.ai[1] = Main.rand.Next(100);
                    return;
                }
                Projectile.localAI[0] += (float)(spinningpoint.X * (double)(Projectile.extraUpdates + 1) * 2.0) * num1;
                Projectile.velocity = spinningpoint.RotatedBy(Projectile.ai[0] + 1.57079637050629, new Vector2()) * num1;
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.570796f;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            for (int index = 0; index < Projectile.oldPos.Length && (Projectile.oldPos[index].X != 0.0 || Projectile.oldPos[index].Y != 0.0); ++index)
            {
                Rectangle myRect = projHitbox;
                myRect.X = (int)Projectile.oldPos[index].X;
                myRect.Y = (int)Projectile.oldPos[index].Y;
                if (myRect.Intersects(targetHitbox))
                    return true;
            }
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            float num2 = (float)(Projectile.rotation + 1.57079637050629 + (Main.rand.NextBool(2)? -1.0 : 1.0) * 1.57079637050629);
            float num3 = (float)(Main.rand.NextDouble() * 2.0 + 2.0);
            Vector2 vector2 = new((float)Math.Cos(num2) * num3, (float)Math.Sin(num2) * num3);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                int index = Dust.NewDust(Projectile.oldPos[i], 0, 0, DustID.Vortex, vector2.X, vector2.Y, 0, new Color(), 1f);
                Main.dust[index].noGravity = true;
                Main.dust[index].scale = 1.7f;
            }
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Lerp(Color.LightSkyBlue, Color.White, 0.5f + (float)Math.Sin(colorlerp) / 2) * 0.5f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture2D13 = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rectangle = texture2D13.Bounds;
            Vector2 origin2 = rectangle.Size() / 2f;
            Color color27 = Projectile.GetAlpha(lightColor);
            for (int i = 1; i < ProjectileID.Sets.TrailCacheLength[Projectile.type]; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero || Projectile.oldPos[i - 1] == Projectile.oldPos[i])
                    continue;
                Vector2 offset = Projectile.oldPos[i - 1] - Projectile.oldPos[i];
                int length = (int)offset.Length();
                float scale = Projectile.scale * (float)Math.Sin(i / MathHelper.Pi);
                offset.Normalize();
                const int step = 3;
                for (int j = 0; j < length; j += step)
                {
                    Vector2 value5 = Projectile.oldPos[i] + offset * j;
                    Main.spriteBatch.Draw(texture2D13, value5 + Projectile.Size / 2f - Main.screenPosition + new Vector2(0, Projectile.gfxOffY), new Microsoft.Xna.Framework.Rectangle?(rectangle), color27, Projectile.rotation, origin2, scale, SpriteEffects.FlipHorizontally, 0f);
                }
            }
            return false;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity = Vector2.Zero;
            return false;
        }
    }
}