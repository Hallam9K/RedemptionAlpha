using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Effects.PrimitiveTrails;
using ParticleLibrary;
using Redemption.Particles;
using Redemption.Dusts;
using Terraria.Audio;

namespace Redemption.Projectiles.Melee
{
    public class NebulaStar : ModProjectile, ITrailProjectile
    {
        public override string Texture => "Redemption/Textures/Sparkle1";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ElementID.ProjShadow[Type] = true;
            ElementID.ProjCelestial[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 360;
            Projectile.scale = Main.rand.NextFloat(1f, 1.2f);
            Projectile.localAI[0] = Main.rand.NextFloat(-0.3f, 0.3f);
        }

        public void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new RainbowTrail(5f, 0.002f, 1f, .75f), new RoundCap(), new DefaultTrailPosition(), 100f, 300f, new ImageShader(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_4", AssetRequestMode.ImmediateLoad).Value, 0.01f, 1f, 1f));
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.rotation += Projectile.localAI[0];
            if (Projectile.timeLeft > 340)
                Projectile.velocity *= 0.98f;
            else
            {
                if (Main.rand.NextBool(30) && Main.myPlayer == Projectile.owner)
                {
                    SoundEngine.PlaySound(SoundID.Item9 with { Volume = .5f }, Projectile.position);
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, RedeHelper.PolarVector(8, RedeHelper.RandomRotation()), ModContent.ProjectileType<NebulaSpark>(), Projectile.damage / 2, 1, player.whoAmI);
                }
                Vector2 move = Vector2.Zero;
                float distance = 900f;
                bool targetted = false;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (!target.CanBeChasedBy() || target.Redemption().invisible)
                        continue;

                    Vector2 newMove = target.Center - Projectile.Center;
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < distance)
                    {
                        move = target.Center;
                        distance = distanceTo;
                        targetted = true;
                    }
                }
                if (targetted)
                    Projectile.Move(move, 5, 80);
                else
                    Projectile.velocity *= 0.98f;
            }
            if (Main.rand.NextBool(10))
                ParticleManager.NewParticle(Projectile.Center, RedeHelper.Spread(2), new RainbowParticle(), Color.White, 0.2f);
        }

        public override bool? CanHitNPC(NPC target) => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Main.DiscoColor * 0.5f, -Projectile.rotation, drawOrigin, Projectile.scale * 2f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 5; i++)
                ParticleManager.NewParticle(Projectile.Center, RedeHelper.Spread(6), new RainbowParticle(), Color.White, 0.3f);
        }
    }
    public class NebulaSpark : ModProjectile, ITrailProjectile
    {
        public override string Texture => "Redemption/Textures/WhiteFlare";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ElementID.ProjShadow[Type] = true;
            ElementID.ProjCelestial[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 180;
            Projectile.scale = Main.rand.NextFloat(0.3f, 0.5f);
        }

        public void DoTrailCreation(TrailManager tManager)
        {
            Color c = RedeColor.NebColour;
            c.A = 0;
            Color c2 = Color.LightPink;
            c.A = 0;
            tManager.CreateTrail(Projectile, new GradientTrail(c, c2), new RoundCap(), new DefaultTrailPosition(), 20f, 160f, new ImageShader(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_4", AssetRequestMode.ImmediateLoad).Value, 0.01f, 1f, 1f));
        }

        public override void AI()
        {
            if (Projectile.timeLeft > 160)
                Projectile.velocity *= 0.98f;
            else
            {
                Vector2 move = Vector2.Zero;
                float distance = 900f;
                bool targetted = false;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC target = Main.npc[i];
                    if (!target.CanBeChasedBy() || target.Redemption().invisible)
                        continue;

                    Vector2 newMove = target.Center - Projectile.Center;
                    float distanceTo = (float)Math.Sqrt(newMove.X * newMove.X + newMove.Y * newMove.Y);
                    if (distanceTo < distance)
                    {
                        move = target.Center;
                        distance = distanceTo;
                        targetted = true;
                    }
                }
                if (targetted)
                    Projectile.Move(move, Projectile.timeLeft > 50 ? 30 : 50, 50);
                else
                    Projectile.velocity *= 0.98f;
            }
        }

        public override bool? CanHitNPC(NPC target) => !target.friendly && Projectile.timeLeft <= 150 ? null : false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, RedeColor.NebColour, Projectile.rotation, drawOrigin, Projectile.scale * 0.8f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            Dust dust = Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), Vector2.Zero, 1);
            dust.noGravity = true;
            Color dustColor = new(RedeColor.NebColour.R, RedeColor.NebColour.G, RedeColor.NebColour.B) { A = 0 };
            dust.color = dustColor;
        }
    }
}
