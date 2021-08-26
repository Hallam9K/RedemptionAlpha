using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Buffs.NPCBuffs;
using Redemption.Dusts;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.Ammo
{
	public class MoonflareArrow_Proj : ModProjectile
	{
        public override string Texture => "Redemption/Items/Weapons/Ammo/MoonflareArrow";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Moonflare Arrow");
			ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		}

		public override void SetDefaults()
		{
			Projectile.width = 14;
			Projectile.height = 14;
			Projectile.friendly = true;
			Projectile.hostile = false;
			Projectile.DamageType = DamageClass.Ranged;
			Projectile.penetrate = 1;
			Projectile.timeLeft = 600;
			Projectile.light = 0.1f;
			Projectile.ignoreWater = false;
			Projectile.tileCollide = true;
			Projectile.extraUpdates = 1;
			AIType = ProjectileID.WoodenArrowFriendly;
		}
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;
            Projectile.velocity.Y += 0.01f;
        }
        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            if (!Main.rand.NextBool(3) || Main.dayTime || Main.moonPhase == 4)
                return;

            target.AddBuff(ModContent.BuffType<MoonflareDebuff>(), 360);
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Dig, (int)Projectile.position.X, (int)Projectile.position.Y, 1);
            return true;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Texture2D textureGlow = ModContent.Request<Texture2D>("Redemption/Items/Weapons/Ammo/MoonflareArrow_Glow").Value;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Color.White * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(texture, drawPos, new Rectangle?(rect), color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), lightColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(textureGlow, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Color.White, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
                Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<MoonflareDust>(), Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
        }
    }
}
