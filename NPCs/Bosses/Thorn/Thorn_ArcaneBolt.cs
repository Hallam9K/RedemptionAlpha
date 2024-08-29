using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.BaseExtension;
using Redemption.Effects;
using Redemption.Globals;
using Redemption.Projectiles;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.Drawing;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Thorn
{
    public class Thorn_ArcaneBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.penetrate = 1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 240;
            Projectile.alpha = 5;
            Projectile.stopsDealingDamageAfterPenetrateHits = true;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            FakeKill();
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            FakeKill();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            FakeKill();
            return false;
        }

        private readonly int NUMPOINTS = 30;
        public Color baseColor = new(211, 246, 236);
        public Color endColor = new(114, 174, 180);
        private List<Vector2> cache;
        private List<Vector2> cache2;
        private DanTrail trail;
        private DanTrail trail2;
        private readonly float thickness = 3f;

        public override void AI()
        {
            if (Projectile.ai[1]++ == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings
                    {
                        PositionInWorld = RedeHelper.RandAreaInEntity(Projectile),
                    });
                }
            }
            if (Projectile.ai[1] < 60)
                Projectile.velocity *= .95f;
            else
            {
                if (Projectile.ai[0] is not 1)
                    Projectile.velocity.Y += .3f;
            }

            if (Projectile.ai[1] == 60)
            {
                SoundEngine.PlaySound(SoundID.Item28, Projectile.position);
                for (int i = 0; i < 10; i++)
                {
                    ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings
                    {
                        PositionInWorld = RedeHelper.RandAreaInEntity(Projectile),
                    });
                }
                Projectile.ai[0] = Main.rand.Next(3);
                Projectile.netUpdate = true;

                if (Projectile.owner == Main.myPlayer && Projectile.ModProjectile is ArcaneBolt)
                {
                    Projectile.velocity = Projectile.ai[0] switch
                    {
                        1 => RedeHelper.PolarVector(Main.rand.Next(10, 15), (Main.MouseWorld - Projectile.Center).ToRotation()),
                        2 => RedeHelper.PolarVector(-Main.rand.Next(10, 15), (Main.MouseWorld - Projectile.Center).ToRotation()),
                        _ => RedeHelper.GetArcVel(Projectile, Main.MouseWorld, .3f, minArcHeight: 80, maxXvel: 8),
                    };
                }
                else
                {
                    NPC npc = Main.npc[(int)Projectile.ai[2]];
                    Entity attacker = Main.player[npc.target];
                    Projectile.velocity = Projectile.ai[0] switch
                    {
                        1 => RedeHelper.PolarVector(Main.rand.Next(10, 15), (attacker.Center - Projectile.Center).ToRotation()),
                        2 => RedeHelper.PolarVector(-Main.rand.Next(10, 15), (attacker.Center - Projectile.Center).ToRotation()),
                        _ => RedeHelper.GetArcVel(Projectile, attacker.Center, .3f, minArcHeight: 80, maxXvel: 8),
                    };
                }
                Projectile.localAI[0] = Main.rand.NextBool() ? 1 : -1;
            }
            if (Projectile.ai[0] is 2 && Projectile.ai[1] > 70 && Projectile.ai[1] < 119)
                Projectile.velocity = Projectile.velocity.RotatedBy(.2f * Projectile.localAI[0]) * 1.01f;

            for (int k = oldPos.Length - 1; k > 0; k--)
                oldPos[k] = oldPos[k - 1];
            oldPos[0] = Projectile.Center;

            if (Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageBasicCaches(ref cache, ref cache2, NUMPOINTS, Projectile.Center + Projectile.velocity);
                TrailHelper.ManageBasicTrail(ref cache, ref cache2, ref trail, ref trail2, NUMPOINTS, Projectile.Center + Projectile.velocity, baseColor, endColor, baseColor, thickness);
            }
            if (fakeTimer > 0)
                FakeKill();
        }
        private int fakeTimer;
        private void FakeKill()
        {
            if (fakeTimer++ == 0)
            {
                SoundEngine.PlaySound(SoundID.Item9 with { Volume = 0.5f }, Projectile.position);
                for (int i = 0; i < 20; i++)
                {
                    int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.MagicMirror, 0, 0, Scale: 2);
                    Main.dust[dust].noGravity = true;
                }
                for (int i = 0; i < 10; i++)
                {
                    ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings
                    {
                        PositionInWorld = RedeHelper.RandAreaInEntity(Projectile),
                    });
                }
            }
            Projectile.alpha = 255;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            if (fakeTimer >= 60)
                Projectile.Kill();
        }

        public Vector2[] oldPos = new Vector2[10];
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:GlowTrailShader"]?.GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_1").Value);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(1f);

            trail?.Render(effect);
            trail2?.Render(effect);

            Main.spriteBatch.BeginDefault();

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            if (fakeTimer != 0)
                return false;

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = oldPos[k] - Main.screenPosition;
                float oldScale = (Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length;
                Color color = Projectile.GetAlpha(lightColor) * oldScale;
                Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, texture.Size() / 2, Projectile.scale * oldScale, 0, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(lightColor), Projectile.rotation, texture.Size() / 2, Projectile.scale, 0, 0);
            return false;
        }
        public override Color? GetAlpha(Color lightColor) => new Color(255, 255, 255, 0);

        public override void OnKill(int timeLeft)
        {
            if (fakeTimer > 0)
                return;
            SoundEngine.PlaySound(SoundID.Item9 with { Volume = 0.5f }, Projectile.position);
            for (int i = 0; i < 20; i++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.MagicMirror, 0, 0, Scale: 2);
                Main.dust[dust].noGravity = true;
            }
            for (int i = 0; i < 10; i++)
            {
                ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.StardustPunch, new ParticleOrchestraSettings
                {
                    PositionInWorld = RedeHelper.RandAreaInEntity(Projectile),
                });
            }
        }
    }
    public class ArcaneBolt : Thorn_ArcaneBolt
    {
        public override string Texture => "Redemption/NPCs/Bosses/Thorn/Thorn_ArcaneBolt";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjArcane[Type] = true;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.Redemption().friendlyHostile = false;
        }
    }
}