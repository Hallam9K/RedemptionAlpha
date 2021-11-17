using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Base;
using Redemption.Dusts;
using Redemption.Globals;
using Redemption.Globals.NPC;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Melee
{
    public class FireSlash_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Heat Wave");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Melee;
            Projectile.timeLeft = 180;
            Projectile.GetGlobalProjectile<RedeProjectile>().Unparryable = true;
        }

        public override void AI()
        {
            Projectile.LookByVelocity();
            if (Projectile.spriteDirection == 1)
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            else
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(135f);

            Projectile.alpha += 8;
            if (Projectile.alpha >= 255)
                Projectile.Kill();
        }

        public override bool? CanHitNPC(NPC target) => !target.friendly && Projectile.alpha <= 200;

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit) => target.AddBuff(BuffID.OnFire, 120);
        public override void OnHitPlayer(Player target, int damage, bool crit) => target.AddBuff(BuffID.OnFire, 120);

        public override bool? CanCutTiles() => false;

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 drawOrigin = new(texture.Width / 2, texture.Height / 2);
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Color.White * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, null, Projectile.GetAlpha(color), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}
