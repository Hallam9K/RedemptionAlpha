using Terraria;
using Terraria.ModLoader;
using Redemption.Buffs;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Terraria.GameContent;
using Redemption.Globals;

namespace Redemption.Textures
{
    public class DreamsongLight_Visual : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Celestine Light");
        }
        public override void SetDefaults()
        {
            Projectile.width = 600;
            Projectile.height = 600;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.alpha = 255;
        }

        NPC npc;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (player.dead || !player.HasBuff(ModContent.BuffType<DreamsongBuff>()))
                Projectile.Kill();

            Projectile.timeLeft = 10;
            Projectile.velocity *= 0;
            Projectile.position = new Vector2(player.Center.X - 300, player.Center.Y - 300);
            for (int p = 0; p < Main.maxNPCs; p++)
            {
                npc = Main.npc[p];
                if (!npc.active || npc.immortal || npc.dontTakeDamage || npc.friendly || !Projectile.Hitbox.Intersects(npc.Hitbox))
                    continue;

                if (!NPCLists.Soulless.Contains(npc.type))
                    continue;

                npc.AddBuff(ModContent.BuffType<DreamsongBuff>(), 10, false);
            }

            Projectile.localAI[0]++;
            if (Projectile.localAI[0] < 60)
            {
                if (Projectile.localAI[0] < 30)
                    Projectile.alpha -= 5;
                else
                    Projectile.alpha += 5;
                Projectile.scale += 0.003f;
            }
            else
            {
                Projectile.alpha = 255;
                Projectile.scale = 1;
                Projectile.localAI[0] = 0;
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, 0, texture.Width, texture.Height);
            Vector2 origin = new(texture.Width / 2f, texture.Height / 2f);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            Main.EntitySpriteDraw(texture, position, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, origin, Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);
            return false;
        }
    }
}