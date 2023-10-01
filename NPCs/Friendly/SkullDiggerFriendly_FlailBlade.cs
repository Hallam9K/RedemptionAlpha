using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Friendly
{
    public class SkullDiggerFriendly_FlailBlade : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Minibosses/SkullDigger/SkullDigger_FlailBlade";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Skull Digger");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }
        public override void SetDefaults()
        {
            Projectile.width = 56;
            Projectile.height = 56;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            NPC host = Main.npc[(int)Projectile.ai[0]];
            if (!host.active || (host.type != ModContent.NPCType<SkullDiggerFriendly>() && host.type != ModContent.NPCType<SkullDiggerFriendly_Spirit>()))
                Projectile.Kill();

            Vector2 originPos = host.Center + new Vector2(host.spriteDirection == 1 ? 35 : -35, -6);
            Vector2 defaultPosition = new(host.Center.X + (host.spriteDirection == 1 ? 35 : -35), host.Center.Y + 60);
            if (host.alpha >= 255)
                Projectile.Center = defaultPosition;

            Projectile.timeLeft = 10;

            Vector2 position = Projectile.Center;
            Vector2 mountedCenter = originPos;
            Vector2 vector2_4 = mountedCenter - position;
            Projectile.rotation = (float)Math.Atan2(vector2_4.Y, vector2_4.X) + 1.57f;
            Projectile.alpha = host.alpha;

            Projectile.Move(defaultPosition, 9, 20);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            if (Projectile.ai[1] == 1)
            {
                int shader = GameShaders.Armor.GetShaderIdFromItemId(ItemID.WispDye);
                Main.spriteBatch.End();
                Main.spriteBatch.BeginAdditive(true);
                GameShaders.Armor.ApplySecondary(shader, Main.LocalPlayer, null);
            }
            NPC host = Main.npc[(int)Projectile.ai[0]];
            Texture2D ballTexture = ModContent.Request<Texture2D>("Redemption/NPCs/Minibosses/SkullDigger/SkullDigger_FlailBlade").Value;
            Vector2 anchorPos = Projectile.Center;
            Texture2D chainTexture = ModContent.Request<Texture2D>("Redemption/NPCs/Minibosses/SkullDigger/SkullDigger_Chain").Value;
            Vector2 HeadPos = host.Center + new Vector2(host.spriteDirection == 1 ? 35 : -35, -6);
            Rectangle sourceRectangle = new(0, 0, chainTexture.Width, chainTexture.Height);
            Vector2 origin = new(chainTexture.Width * 0.5f, chainTexture.Height * 0.5f);
            float num1 = chainTexture.Height;
            Vector2 vector2_4 = anchorPos - HeadPos;
            var effects = host.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
            float rotation = (float)Math.Atan2(vector2_4.Y, vector2_4.X) - 1.57f;
            bool flag = true;
            if (float.IsNaN(HeadPos.X) && float.IsNaN(HeadPos.Y))
                flag = false;
            if (float.IsNaN(vector2_4.X) && float.IsNaN(vector2_4.Y))
                flag = false;
            while (flag)
            {
                if (vector2_4.Length() < num1 + 1.0)
                    flag = false;
                else
                {
                    Vector2 vector2_1 = vector2_4;
                    vector2_1.Normalize();
                    HeadPos += vector2_1 * num1;
                    vector2_4 = anchorPos - HeadPos;
                    Main.EntitySpriteDraw(chainTexture, HeadPos - Main.screenPosition, new Rectangle?(sourceRectangle), Projectile.GetAlpha(Projectile.ai[1] == 1 ? Color.White : lightColor), rotation, origin, 1, SpriteEffects.None, 0);
                }
            }
            Vector2 position = Projectile.Center - Main.screenPosition;
            Rectangle rect = new(0, 0, ballTexture.Width, ballTexture.Height);
            Vector2 origin2 = new(ballTexture.Width / 2, ballTexture.Height / 2);

            for (int k = 0; k < Projectile.oldPos.Length; k++)
            {
                Vector2 drawPos = Projectile.oldPos[k] + Projectile.Size / 2f - Main.screenPosition;
                Color color = Projectile.GetAlpha(Color.LightCyan) * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                Main.EntitySpriteDraw(ballTexture, drawPos, new Rectangle?(rect), color, Projectile.rotation, origin2, Projectile.scale, effects, 0);
            }

            Main.EntitySpriteDraw(ballTexture, position, new Rectangle?(rect), Projectile.GetAlpha(Projectile.ai[1] == 1 ? Color.White : lightColor), Projectile.rotation, origin2, Projectile.scale, effects, 0);
            if (Projectile.ai[1] == 1)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.BeginDefault();
            }
            return false;
        }
    }
}