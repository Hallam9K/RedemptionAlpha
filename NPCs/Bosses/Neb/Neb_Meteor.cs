using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using ParticleLibrary.Core;
using Redemption.BaseExtension;
using Redemption.Effects;
using Redemption.Globals;
using Redemption.Particles;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Neb
{
    public class Neb_Meteor_Tele : ModProjectile
    {
        public override string Texture => "Redemption/Textures/RadialTelegraph3";
        public override void SetDefaults()
        {
            Projectile.width = 400;
            Projectile.height = 400;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.alpha = 255;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            switch (Projectile.localAI[0])
            {
                case 0:
                    Projectile.alpha -= 4;
                    if (Projectile.alpha <= 50)
                    {
                        SoundEngine.PlaySound(SoundID.Item88 with { Pitch = -.5f }, Projectile.Center);
                        if (Projectile.owner == Main.myPlayer)
                            Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center + new Vector2(Main.rand.Next(-200, 201), -Main.rand.Next(2000, 2201)), Vector2.Zero, ProjectileType<Neb_Meteor>(), Projectile.damage, Projectile.knockBack, Main.myPlayer, Projectile.whoAmI, Projectile.ai[1]);
                        Projectile.localAI[0]++;
                        Projectile.netUpdate = true;
                    }
                    break;
            }

            if (Projectile.ai[0]++ > 60)
                Projectile.ai[0] = 0;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D circle = TextureAssets.Projectile[Type].Value;
            Rectangle rect = new(0, 0, circle.Width, circle.Height);
            Vector2 origin = new(circle.Width / 2, circle.Height / 2);
            Color colour = Color.Orange;
            Vector2 position = Projectile.Center - Main.screenPosition;
            float scale;
            float opacity;

            if (Projectile.ai[0] <= 30)
            {
                scale = Projectile.ai[0] / 32;
                opacity = 1f;
            }
            else if (Projectile.ai[0] < 60)
            {
                opacity = 1f - (Projectile.ai[0] - 30) / 30;
                scale = 0.9375f + (Projectile.ai[0] - 30) / 320;
            }
            else scale = opacity = 0f;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, default, Main.GameViewMatrix.TransformationMatrix);

            float normalScale = .7f;
            if (Projectile.ai[1] is 1)
                normalScale = 1f;

            Main.EntitySpriteDraw(circle, position, new Rectangle?(rect), Projectile.GetAlpha(colour), Projectile.rotation - MathHelper.PiOver2, origin, normalScale, 0, 0);
            Main.EntitySpriteDraw(circle, position, new Rectangle?(rect), Projectile.GetAlpha(colour) * opacity, Projectile.rotation - MathHelper.PiOver2, origin, normalScale * scale, 0, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
    public class Neb_Meteor : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            ElementID.ProjFire[Type] = true;
            ElementID.ProjExplosive[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 900;
            Projectile.scale = 2;
            meteorID = Main.rand.Next(3) switch
            {
                1 => ProjectileID.Meteor2,
                2 => ProjectileID.Meteor3,
                _ => (int)ProjectileID.Meteor1,
            };
        }
        private int meteorID;
        private readonly int NUMPOINTS = 30;
        public Color baseColor = new(230, 90, 0);
        public Color endColor = new(253, 154, 45);
        public Color edgeColor = new(255, 236, 98);
        private List<Vector2> cache;
        private List<Vector2> cache2;
        private DanTrail trail;
        private DanTrail trail2;
        private readonly float thickness = 30f;
        public override bool CanHitPlayer(Player target) => true;
        public override bool? CanHitNPC(NPC target) => null;
        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:GlowTrailShader"]?.GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(Request<Texture2D>("Redemption/Textures/Trails/Trail_1").Value);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(1f);

            trail?.Render(effect);
            trail2?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();

            Texture2D meteor = Request<Texture2D>("Terraria/Images/Projectile_" + meteorID, AssetRequestMode.ImmediateLoad).Value;
            Rectangle rect = new(0, 0, meteor.Width, meteor.Height);
            Vector2 origin = new(meteor.Width / 2, meteor.Height / 2);
            Main.EntitySpriteDraw(meteor, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Color.White, Projectile.rotation, origin, Projectile.scale, 0);
            return true;
        }
        public override void AI()
        {
            Projectile proj = Main.projectile[(int)Projectile.ai[0]];
            if (!proj.active || proj.type != ProjectileType<Neb_Meteor_Tele>())
                Projectile.Kill();

            int speed = 25;
            if (Projectile.ai[1] is 1)
                speed = 38;
            Projectile.Move(proj.Center, speed, 1);
            if (Projectile.DistanceSQ(proj.Center) < 50 * 50)
            {
                proj.Kill();
                Projectile.Kill();
            }

            Vector2 position = Projectile.Center + (Vector2.Normalize(Projectile.velocity) * 10f);
            Dust dust20 = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch)];
            dust20.position = position;
            dust20.velocity = (Projectile.velocity.RotatedBy(MathHelper.PiOver2) * 0.33f) + (Projectile.velocity / 4f);
            dust20.position += Projectile.velocity.RotatedBy(MathHelper.PiOver2);
            dust20.fadeIn = 0.5f;
            dust20.noGravity = true;
            dust20 = Main.dust[Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch)];
            dust20.position = position;
            dust20.velocity = (Projectile.velocity.RotatedBy(-MathHelper.PiOver2) * 0.33f) + (Projectile.velocity / 4f);
            dust20.position += Projectile.velocity.RotatedBy(-MathHelper.PiOver2);
            dust20.fadeIn = 0.5f;
            dust20.noGravity = true;

            if (Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageBasicCaches(ref cache, ref cache2, NUMPOINTS, Projectile.Center + Projectile.velocity);
                TrailHelper.ManageBasicTrail(ref cache, ref cache2, ref trail, ref trail2, NUMPOINTS, Projectile.Center + Projectile.velocity, baseColor, endColor, edgeColor, thickness);
            }
        }
        public override void OnKill(int timeLeft)
        {
            Projectile.hostile = true;
            if (!Main.dedServ)
                SoundEngine.PlaySound(CustomSounds.MissileExplosion with { Pitch = -.5f }, Projectile.Center);
            SoundEngine.PlaySound(SoundID.Item89 with { Volume = 2 }, Projectile.Center);
            Main.LocalPlayer.RedemptionScreen().ScreenShakeOrigin = Projectile.Center;
            Main.LocalPlayer.RedemptionScreen().ScreenShakeIntensity += 12;
            if (Projectile.ai[1] is 1)
            {
                RedeDraw.SpawnExplosion(Projectile.Center, Color.OrangeRed, scale: 2);
                for (int i = 0; i < 20; i++)
                    ParticleManager.NewParticle(Projectile.Center, RedeHelper.Spread(16), new EmberParticle(), Color.White, 5, 0, 0, 2);
            }
            else
            {
                RedeDraw.SpawnExplosion(Projectile.Center, Color.OrangeRed);
                for (int i = 0; i < 20; i++)
                    ParticleManager.NewParticle(Projectile.Center, RedeHelper.Spread(10), new EmberParticle(), Color.White, 3, 0, 0, 2);
            }

            for (int i = 0; i < 24; i++)
            {
                int dust2 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Torch, 0, 0, Scale: 2);
                Main.dust[dust2].velocity *= 10f;
                Main.dust[dust2].noGravity = true;
            }
            if (Main.netMode != NetmodeID.Server)
            {
                for (int g = 0; g < 3; g++)
                {
                    int goreIndex = Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.Center, default, Main.rand.Next(61, 64));
                    Main.gore[goreIndex].velocity.X = Main.gore[goreIndex].velocity.X + 1.5f;
                    Main.gore[goreIndex].velocity.Y = Main.gore[goreIndex].velocity.Y + 1.5f;
                }
            }
            int radius = 148;
            if (Projectile.ai[1] is 1)
                radius = 190;
             RedeHelper.PlayerRadiusDamage(radius, Projectile, NPCHelper.HostileProjDamageInc(Projectile.damage), Projectile.knockBack);
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.ArmorPenetration += 50;
            target.AddBuff(BuffID.OnFire, 600);
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.Knockback.Base += 6;
            modifiers.ScalingArmorPenetration += .5f;
            target.AddBuff(BuffID.OnFire, 600);
        }
    }
}