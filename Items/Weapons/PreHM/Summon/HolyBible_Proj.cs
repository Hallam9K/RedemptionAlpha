using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Redemption.Projectiles.Minions;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Summon
{
    public class HolyBible_Proj : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 32;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.velocity *= -1;
            if (Projectile.ai[0] < 30)
                Projectile.ai[0] = 30;
            return false;
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 16;
            return true;
        }
        private float glowRot;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            glowRot += 0.04f;

            if (Projectile.ai[0]++ >= 30 && Projectile.ai[0] <= 240)
            {
                Projectile.velocity *= 0.9f;
                Projectile.rotation.SlowRotation(0, (float)Math.PI / 20);
            }
            else if (Projectile.owner == player.whoAmI)
            {
                if (Projectile.ai[0] < 30)
                {
                    Projectile.timeLeft = 600;
                    Projectile.ai[0] = 0;
                    Projectile.Move(Main.MouseWorld, 10, 10);
                    if (Projectile.DistanceSQ(Main.MouseWorld) < 60 * 60)
                        Projectile.ai[0] = 30;
                }
                Projectile.LookByVelocity();
                Projectile.rotation += Projectile.velocity.Length() / 50 * Projectile.spriteDirection;
            }
            if (Projectile.ai[0] == 60 && Main.myPlayer == Projectile.owner)
            {
                Projectile.ai[1] = 10;
                SoundEngine.PlaySound(SoundID.Item122, Projectile.position);
                for (int i = 0; i < 4; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.PolarVector(2, MathHelper.PiOver2 * i),
                        ModContent.ProjectileType<HolyBible_Ray>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.whoAmI);
            }
            if (Projectile.ai[0] >= 240)
            {
                Projectile.tileCollide = false;
                Projectile.Move(player.Center, Projectile.ai[1], 1);
                Projectile.ai[1] *= 1.01f;
                if (Projectile.DistanceSQ(player.Center) < 20 * 20)
                    Projectile.Kill();
            }
        }

        public override void PostDraw(Color lightColor)
        {
            Texture2D glowTex = ModContent.Request<Texture2D>("Redemption/Textures/Star").Value;
            Rectangle rectGlow = new(0, 0, glowTex.Width, glowTex.Height);
            Vector2 drawOriginGlow = new(glowTex.Width / 2, glowTex.Height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            if (Projectile.ai[0] >= 60 && Projectile.ai[0] <= 240)
            {
                Main.EntitySpriteDraw(glowTex, Projectile.Center - Main.screenPosition, new Rectangle?(rectGlow), Projectile.GetAlpha(Color.Yellow) * 0.6f, glowRot, drawOriginGlow, Projectile.scale, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(glowTex, Projectile.Center - Main.screenPosition, new Rectangle?(rectGlow), Projectile.GetAlpha(Color.Yellow) * 0.4f, -glowRot, drawOriginGlow, Projectile.scale, SpriteEffects.None, 0);
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
        }
    }
}