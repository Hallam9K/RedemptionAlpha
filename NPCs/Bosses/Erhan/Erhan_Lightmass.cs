using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Effects;
using System.Collections.Generic;

namespace Redemption.NPCs.Bosses.Erhan
{
    public class Erhan_Lightmass : ModProjectile
    {
        public override string Texture => "Redemption/Textures/WhiteFlare";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Lightmass");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
            ElementID.ProjHoly[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 180;
            Projectile.scale = Main.rand.NextFloat(0.5f, 1);
        }

        private readonly int NUMPOINTS = 20;
        public Color baseColor = new(255, 255, 120);
        public Color endColor = Color.White;
        public Color edgeColor = new(255, 255, 120);
        private List<Vector2> cache;
        private List<Vector2> cache2;
        private DanTrail trail;
        private DanTrail trail2;
        private float thickness = 1f;

        private Vector2 origin;
        public override void AI()
        {
            thickness = Projectile.scale;
            if (Projectile.timeLeft >= 150)
            {
                Projectile.velocity *= 0.98f;
                origin = Projectile.Center;
            }
            else if (Projectile.timeLeft >= 100 && Projectile.timeLeft < 150)
            {
                if (Projectile.ai[0] is 1)
                    Projectile.Move(origin + new Vector2(0, 300), 30, 50);
                else
                {
                    Vector2 move = Vector2.Zero;
                    float distance = 4000f;
                    bool targetted = false;
                    for (int p = 0; p < Main.maxPlayers; p++)
                    {
                        Player target = Main.player[p];
                        if (!target.active || target.dead || target.invis || !Collision.CanHit(Projectile.Center, 0, 0, target.Center, 0, 0))
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
                        Projectile.Move(move, 30, 50);
                    else
                        Projectile.velocity *= 0.98f;
                }
            }
            if (Main.netMode != NetmodeID.Server)
            {
                TrailHelper.ManageBasicCaches(ref cache, ref cache2, NUMPOINTS, Projectile.Center + Projectile.velocity);
                TrailHelper.ManageBasicTrail(ref cache, ref cache2, ref trail, ref trail2, NUMPOINTS, Projectile.Center + Projectile.velocity, baseColor, endColor, edgeColor, thickness);
            }
        }

        public override bool CanHitPlayer(Player target) => Projectile.timeLeft <= 150;

        public override bool PreDraw(ref Color lightColor)
        {
            Main.spriteBatch.End();
            Effect effect = Terraria.Graphics.Effects.Filters.Scene["MoR:GlowTrailShader"]?.GetShader().Shader;

            Matrix world = Matrix.CreateTranslation(-Main.screenPosition.Vec3());
            Matrix view = Main.GameViewMatrix.ZoomMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);

            effect.Parameters["transformMatrix"].SetValue(world * view * projection);
            effect.Parameters["sampleTexture"].SetValue(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_4").Value);
            effect.Parameters["time"].SetValue(Main.GameUpdateCount * 0.05f);
            effect.Parameters["repeats"].SetValue(1f);

            trail?.Render(effect);
            trail2?.Render(effect);

            Main.spriteBatch.Begin(default, default, default, default, default, default, Main.GameViewMatrix.ZoomMatrix);

            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 120), Projectile.rotation, drawOrigin, Projectile.scale * 0.8f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GoldFlame, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
