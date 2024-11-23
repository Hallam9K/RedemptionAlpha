using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Helpers;
using ReLogic.Content;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Magic
{
    public class GigapeiliDrone : ModProjectile
    {
        public override string Texture => "Redemption/Projectiles/Minions/MicroshieldDrone";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gigapeili Drone");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 38;
            Projectile.height = 38;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = -1;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 3600;
            Projectile.localAI[0] = 10;
        }
        private NPC target;
        public float Timer;
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
            Lighting.AddLight(Projectile.Center, Projectile.Opacity * 0.7f, 0f, 0f);
            Player player = Main.player[Projectile.owner];
            if (player.dead || !player.active)
                Projectile.Kill();

            if (Projectile.ai[0] is 1)
            {
                Projectile.localAI[0]++;
                Projectile.rotation.SlowRotation((player.Center - Projectile.Center).ToRotation(), MathHelper.Pi / 90);
                Projectile.Move(player.Center, Projectile.localAI[0], 1);
                if (Projectile.Hitbox.Intersects(player.Hitbox))
                {
                    if (!Main.dedServ)
                        SoundEngine.PlaySound(CustomSounds.ShootChange, player.position);
                    Projectile.Kill();
                }
                return;
            }
            if (Projectile.timeLeft < 90)
                Projectile.ai[0] = 1;
            if (Projectile.timeLeft > 3540)
            {
                Projectile.rotation = Projectile.localAI[0];
                Projectile.localAI[0] *= 0.9f;
            }
            else
            {
                if (RedeHelper.ClosestNPC(ref target, 2000, Projectile.Center))
                    Projectile.rotation.SlowRotation((Projectile.Center - target.Center).ToRotation() - MathHelper.PiOver2, MathHelper.Pi / 10);
                for (int j = 0; j < Main.maxProjectiles; j++)
                {
                    Projectile proj = Main.projectile[j];
                    if (!proj.active || proj.type != ModContent.ProjectileType<GigapeiliBolt>() || proj.ai[0] == 1)
                        continue;
                    if (!Helper.CheckCircularCollision(Projectile.Center, 80, proj.Hitbox))
                        continue;

                    for (int i = 0; i < 3; i++)
                    {
                        int dust = Dust.NewDust(proj.Center - Vector2.One, 1, 1, ModContent.DustType<GlowDust>(), 0, 0, 0, default, .5f);
                        Main.dust[dust].noGravity = true;
                        Main.dust[dust].velocity *= 0;
                        Color dustColor = new(255, 176, 70) { A = 0 };
                        Main.dust[dust].color = dustColor;
                    }
                    SoundEngine.PlaySound(SoundID.NPCHit34, Projectile.position);
                    proj.velocity = -proj.velocity * 2;
                    proj.damage = (int)(proj.damage * 1.5f);
                    proj.timeLeft = 120;
                    proj.ai[0] = 1;
                }
            }
            Projectile.velocity *= .96f;
            if (Timer++ > 60)
                Timer = 0;
        }

        public static float c = 1f / 255f;
        public Color innerColor = new(150 * c * 0.5f, 20 * c * 0.5f, 54 * c * 0.5f, 1f);
        public Color borderColor = new(215 * c, 79 * c, 214 * c, 1f);

        public override bool PreDraw(ref Color lightcolor)
        {
            //example apply of shader on sprite, not on texture
            Main.spriteBatch.End();
            Texture2D texture = ModContent.Request<Texture2D>("Redemption/Textures/PlainCircle").Value;
            var effects = Projectile.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2, texture.Height / 2);

            Effect ShieldEffect = ModContent.Request<Effect>("Redemption/Effects/Shield", AssetRequestMode.ImmediateLoad).Value;
            Texture2D HexagonTexture = ModContent.Request<Texture2D>("Redemption/Textures/Hexagons", AssetRequestMode.ImmediateLoad).Value;
            Vector2 pos = Projectile.Center - Main.screenPosition;

            ShieldEffect.Parameters["offset"].SetValue(new Vector2(0.2f, 0f));
            ShieldEffect.Parameters["sampleTexture"].SetValue(HexagonTexture);
            ShieldEffect.Parameters["time"].SetValue(Main.GlobalTimeWrappedHourly * 6);
            ShieldEffect.Parameters["border"].SetValue(Color.Multiply(borderColor, .5f).ToVector4());
            ShieldEffect.Parameters["inner"].SetValue(Color.Multiply(innerColor, .5f).ToVector4());
            ShieldEffect.Parameters["sinMult"].SetValue(3f);
            ShieldEffect.Parameters["spriteRatio"].SetValue(new Vector2(3f, 3f));
            ShieldEffect.Parameters["conversion"].SetValue(new Vector2(1f / (texture.Width / 2), 1f / (texture.Height / 2)));
            ShieldEffect.Parameters["frameAmount"].SetValue(1f);
            Main.spriteBatch.BeginAdditive(true);
            ShieldEffect.CurrentTechnique.Passes[0].Apply();
            if (!(Projectile.ai[0] == 1))
            {
                Main.EntitySpriteDraw(texture, pos, rect, Color.Red * 0.2f, 0, origin, 0.82f, effects, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D circle = ModContent.Request<Texture2D>("Redemption/Textures/RadialTelegraph3", AssetRequestMode.ImmediateLoad).Value;
            Texture2D conical = ModContent.Request<Texture2D>("Redemption/Textures/RadialTelegraph2", AssetRequestMode.ImmediateLoad).Value;

            Rectangle rect = new(0, 0, circle.Width, circle.Height);
            Vector2 origin = new(circle.Width / 2, circle.Height / 2);

            Color colour = new(255, 0, 0);
            Vector2 position = Projectile.Center - Main.screenPosition;
            float scale;
            float opacity;

            if (Timer <= 30)
            {
                scale = Timer / 32;
                opacity = 1f;
            }
            else if (Timer < 60)
            {
                opacity = 1f - (Timer - 30) / 30;
                scale = 0.9375f + (Timer - 30) / 320;
            }
            else scale = opacity = 0f;

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            if (!(Projectile.ai[0] == 1))
            {
                Main.EntitySpriteDraw(circle, position, new Rectangle?(rect), colour, Projectile.rotation - MathHelper.PiOver2, origin, 0.36f, 0, 0);
                Main.EntitySpriteDraw(circle, position, new Rectangle?(rect), colour * opacity, Projectile.rotation - MathHelper.PiOver2, origin, 0.37f * scale, 0, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            if (!(Projectile.ai[0] == 1))
            {
                Main.EntitySpriteDraw(conical, position, new Rectangle?(rect), colour * 0.5f, Projectile.rotation - MathHelper.PiOver2, origin, 0.34f, 0, 0);
                Main.EntitySpriteDraw(conical, position, new Rectangle?(rect), colour * 0.5f, Projectile.rotation - 3 * MathHelper.PiOver4 + 0.08f, origin, 0.34f, 0, 0);
                Main.EntitySpriteDraw(conical, position, new Rectangle?(rect), colour * 0.5f, Projectile.rotation - MathHelper.PiOver4 - 0.08f, origin, 0.34f, 0, 0);
            }
        }
    }
}