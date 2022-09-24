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

namespace Redemption.NPCs.Bosses.Erhan
{
    public class Erhan_Lightmass : ModProjectile, ITrailProjectile
    {
        public override string Texture => "Redemption/Textures/WhiteFlare";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Lightmass");
            ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
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

        public void DoTrailCreation(TrailManager tManager)
        {
            tManager.CreateTrail(Projectile, new GradientTrail(new Color(255, 255, 120), Color.White), new RoundCap(), new DefaultTrailPosition(), 50f, 80f, new ImageShader(ModContent.Request<Texture2D>("Redemption/Textures/Trails/Trail_4", AssetRequestMode.ImmediateLoad).Value, 0.01f, 1f, 1f));
        }

        public override void AI()
        {
            if (Projectile.timeLeft >= 150)
                Projectile.velocity *= 0.98f;
            else if (Projectile.timeLeft >= 100 && Projectile.timeLeft < 150)
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

        public override bool CanHitPlayer(Player target) => Projectile.timeLeft <= 150;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, new Color(255, 255, 120), Projectile.rotation, drawOrigin, Projectile.scale * 0.8f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GoldFlame, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust].noGravity = true;
            }
        }
    }
}
