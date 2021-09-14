using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Dusts;
using Redemption.Globals;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Melee
{
    public class Lightmass : ModProjectile
    {
        public override string Texture => "Redemption/Textures/WhiteFlare";
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.CountsAsHoming[Projectile.type] = true;
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
            Projectile.scale = Main.rand.NextFloat(0.5f, 1);
            Projectile.GetGlobalProjectile<RedeGlobalProjectile>().Unparryable = true;
        }

        public override void AI()
        {
            if (Projectile.timeLeft > 150)
                Projectile.velocity *= 0.98f;
            else
            {
                Vector2 move = Vector2.Zero;
                float distance = 900f;
                bool targetted = false;
                foreach (NPC target in Main.npc.Take(Main.maxNPCs))
                {
                    if (!target.CanBeChasedBy() || !Collision.CanHit(Projectile.Center, 0, 0, target.Center, 0, 0))
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

        public override bool? CanHitNPC(NPC target) => Projectile.timeLeft <= 150;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), new Color(255, 255, 120), Projectile.rotation, drawOrigin, Projectile.scale * 0.8f, SpriteEffects.None, 0);

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
