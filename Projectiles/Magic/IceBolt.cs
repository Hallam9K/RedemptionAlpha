using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;
using Redemption.Effects;
using System.Collections.Generic;
using Redemption.Buffs.Debuffs;

namespace Redemption.Projectiles.Magic
{
    public class IceBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ElementID.ProjIce[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 5;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
        }

        private readonly int NUMPOINTS = 40;
        public Color baseColor = Color.LightBlue;
        public Color endColor = new(200, 223, 230);
        public Color edgeColor = Color.LightBlue;
        private List<Vector2> cache;
        private List<Vector2> cache2;
        private DanTrail trail;
        private DanTrail trail2;
        private float thickness = 3;

        private float glowRot;
        private bool fail;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile staff = Main.projectile[(int)Projectile.ai[1]];
            glowRot += 0.04f;
            thickness = 3 * Projectile.scale;
            if (player.channel && Projectile.ai[0] == 0)
            {
                float num = MathHelper.ToRadians(0f);
                Vector2 vector = player.RotatedRelativePoint(player.MountedCenter, true);
                if (Projectile.spriteDirection == -1)
                    num = MathHelper.ToRadians(90f);

                if (Main.myPlayer == Projectile.owner)
                {
                    float scaleFactor6 = 1f;
                    if (player.inventory[player.selectedItem].shoot == Projectile.type)
                    {
                        scaleFactor6 = player.inventory[player.selectedItem].shootSpeed * Projectile.scale;
                    }
                    Vector2 vector13 = Main.MouseWorld - vector;
                    vector13.Normalize();
                    if (vector13.HasNaNs())
                    {
                        vector13 = Vector2.UnitX * player.direction;
                    }
                    vector13 *= scaleFactor6;
                    if (vector13.X != Projectile.velocity.X || vector13.Y != Projectile.velocity.Y)
                        Projectile.netUpdate = true;

                    Projectile.velocity = vector13;
                    if (player.noItems || player.CCed || player.dead || !player.active)
                    {
                        Projectile.Kill();
                    }
                    Projectile.netUpdate = true;
                }
                Vector2 Offset = Vector2.Normalize(staff.velocity) * 10f;

                if (!Collision.CanHit(player.Center, 0, 0, Projectile.Center + Offset, 0, 0))
                    fail = true;
                else
                    fail = false;
                Projectile.position = player.RotatedRelativePoint(player.MountedCenter + RedeHelper.PolarVector(35, Projectile.velocity.ToRotation()), true) - Projectile.Size / 2f;
                Projectile.rotation = Projectile.velocity.ToRotation() + num;
                Projectile.spriteDirection = Projectile.direction;

                Projectile.timeLeft = 180;
                Projectile.scale += 0.05f;
                Projectile.scale = MathHelper.Clamp(Projectile.scale, 1, 2.5f);
            }
            else if (Projectile.ai[0] == 0)
            {
                if (fail)
                    Projectile.Kill();
                SoundEngine.PlaySound(SoundID.Item43, player.position);
                DustHelper.DrawCircle(Projectile.Center, DustID.IceTorch, 2, 2, 2, 1, 3, nogravity: true);
                Projectile.tileCollide = true;
                Projectile.friendly = true;
                Projectile.velocity = RedeHelper.PolarVector(12, Projectile.velocity.ToRotation());
                Projectile.ai[0] = 1;
            }
            else
            {
                if (Main.netMode != NetmodeID.Server)
                {
                    TrailHelper.ManageBasicCaches(ref cache, ref cache2, NUMPOINTS, Projectile.Center + Projectile.velocity);
                    TrailHelper.ManageBasicTrail(ref cache, ref cache2, ref trail, ref trail2, NUMPOINTS, Projectile.Center + Projectile.velocity, baseColor, endColor, edgeColor, thickness);
                }
                Projectile.LookByVelocity();
                Projectile.rotation += Projectile.velocity.Length() / 50 * Projectile.spriteDirection;
            }
            if (fakeTimer > 0)
                FakeKill();
        }
        private int fakeTimer;
        private void FakeKill()
        {
            if (fakeTimer++ == 0)
            {
                SoundEngine.PlaySound(SoundID.Item50, Projectile.position);
                DustHelper.DrawCircle(Projectile.Center, DustID.IceTorch, 2 * Projectile.scale, 1, 1, 1, 3, nogravity: true);
            }
            Projectile.alpha = 255;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.velocity *= 0;
            Projectile.timeLeft = 2;
            Projectile.tileCollide = false;
            if (fakeTimer >= 60)
                Projectile.Kill();
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            FakeKill();
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            if (fakeTimer > 0)
                return;

            SoundEngine.PlaySound(SoundID.Item50, Projectile.position);
            DustHelper.DrawCircle(Projectile.Center, DustID.IceTorch, 2 * Projectile.scale, 1, 1, 1, 3, nogravity: true);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:GlowTrailShader"]?.GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_3").Value);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(1f);

            trail?.Render(effect);
            trail2?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D glowTex = ModContent.Request<Texture2D>("Redemption/Textures/Star").Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            Rectangle rectGlow = new(0, 0, glowTex.Width, glowTex.Height);
            Vector2 drawOriginGlow = new(glowTex.Width / 2, glowTex.Height / 2);
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale * 0.5f, effects, 0);

            if (Projectile.ai[0] == 0)
            {
                Main.EntitySpriteDraw(glowTex, Projectile.Center - Main.screenPosition, new Rectangle?(rectGlow), Projectile.GetAlpha(Color.LightBlue) * 0.6f, glowRot, drawOriginGlow, Projectile.scale * 0.7f, effects, 0);
                Main.EntitySpriteDraw(glowTex, Projectile.Center - Main.screenPosition, new Rectangle?(rectGlow), Projectile.GetAlpha(Color.LightBlue) * 0.4f, -glowRot, drawOriginGlow, Projectile.scale * 0.7f, effects, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.penetrate <= 1)
                FakeKill();

            Player player = Main.player[Projectile.owner];

            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.Frostburn, 240);

            if (player.RedemptionPlayerBuff().pureIronBonus)
                target.AddBuff(ModContent.BuffType<PureChillDebuff>(), 300);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            if (Main.rand.NextBool(3))
                target.AddBuff(BuffID.Frostburn, 240);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            float newDamage = Projectile.scale - 1;
            modifiers.FinalDamage *= (newDamage * 1.5f) + 1;
        }
        public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers) => modifiers.FinalDamage *= Projectile.scale;
    }
}