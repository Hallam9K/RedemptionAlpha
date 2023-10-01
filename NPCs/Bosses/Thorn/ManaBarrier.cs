using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.Thorn
{
    public class ManaBarrier : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 70;
            Projectile.height = 70;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                {
                    Projectile.frame = 0;
                }
            }

            NPC host = Main.npc[(int)Projectile.ai[0]];
            if (!host.active || host.type != ModContent.NPCType<Thorn>())
                Projectile.Kill();
            Projectile.timeLeft = 10;
            Projectile.rotation = (host.Center - Projectile.Center).ToRotation();

            if (host.ai[0] == 3 || host.ai[0] == 6)
                Projectile.alpha += 10;
            else if (host.ai[0] == 4)
                Projectile.alpha -= 10;
            else
                Projectile.alpha = 0;

            Projectile.localAI[0] += 2;
            Projectile.Center = host.Center + Vector2.One.RotatedBy(MathHelper.ToRadians(Projectile.localAI[0])) * 100;

            if (Projectile.alpha < 100)
            {
                for (int i = 0; i < Main.maxProjectiles; i++)
                {
                    Projectile target = Main.projectile[i];
                    if (!target.active || target.whoAmI == Projectile.whoAmI || target.hostile || target.damage > 100)
                        continue;

                    if (target.velocity.Length() == 0 || !Projectile.Hitbox.Intersects(target.Hitbox) || target.ProjBlockBlacklist(true))
                        continue;

                    SoundEngine.PlaySound(SoundID.Item29, Projectile.position);
                    DustHelper.DrawCircle(target.Center, DustID.MagicMirror, 1, 4, 4, nogravity: true);
                    if (target.DamageType != DamageClass.Magic)
                    {
                        target.Kill();
                        return;
                    }
                    if (!target.hostile && target.friendly)
                    {
                        target.hostile = true;
                    }
                    target.damage /= 4;
                    target.velocity = -target.velocity;
                }
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 3;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}