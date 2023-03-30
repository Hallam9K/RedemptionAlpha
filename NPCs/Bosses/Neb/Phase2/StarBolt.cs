using Microsoft.Xna.Framework;
using Terraria;
using System;
using Terraria.ModLoader;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using Terraria.GameContent;
using Redemption.Globals;

namespace Redemption.NPCs.Bosses.Neb.Phase2
{
    public class StarBolt : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Star Bolt");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
            ElementID.ProjCelestial[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 300;
            Projectile.extraUpdates = 1;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }
        public override bool? CanCutTiles() => false;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            Projectile.alpha -= 10;
            Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + 1.57f;
            Projectile.velocity *= 1.01f;
            if (Projectile.localAI[0] == 0 && Main.myPlayer == Projectile.owner)
            {
                if (Main.myPlayer == Projectile.owner)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<NebTeleLine2>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                Projectile.localAI[0] = 1;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2, texture.Height / 2);
            Vector2 drawOrigin = new Vector2(TextureAssets.Projectile[Projectile.type].Value.Width * 0.5f, Projectile.height * 0.5f);
            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
                Color color = Projectile.GetAlpha(Main.DiscoColor) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(TextureAssets.Projectile[Projectile.type].Value, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);
            }
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), lightColor, Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            Texture2D flare = ModContent.Request<Texture2D>("Redemption/Textures/Star").Value;
            Rectangle rect2 = new(0, 0, flare.Width, flare.Height);
            Vector2 origin2 = new(flare.Width / 2, flare.Height / 2);
            Vector2 position2 = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(flare, position2, new Rectangle?(rect2), Main.DiscoColor, Projectile.rotation, origin2, 1f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(flare, position2, new Rectangle?(rect2), Main.DiscoColor * 0.4f, Projectile.rotation + MathHelper.PiOver4, origin2, 1f, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}