using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Redemption.Globals;

namespace Redemption.NPCs.Bosses.Gigapora
{
    public class Gigapora_BoltTele : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Energy Bolt");
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 240;
            Projectile.alpha = 240;
        }
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active || npc.type != ModContent.NPCType<Gigapora_BodySegment>() || npc.ai[0] != 1)
                Projectile.Kill();

            Projectile.Center = npc.Center + RedeHelper.PolarVector(36 * Projectile.ai[1], npc.rotation) + RedeHelper.PolarVector(18, npc.rotation + MathHelper.PiOver2);
            if (Projectile.localAI[1] == 1)
            {
                Projectile.alpha += 20;
                if (Projectile.alpha >= 255)
                    Projectile.Kill();
                return;
            }
            Projectile.localAI[0] += 10;
            Projectile.alpha -= 4;
            if (Projectile.alpha <= 0)
                Projectile.localAI[1] = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (Projectile.ai[0] > -1)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.BeginAdditive();

                Main.EntitySpriteDraw(ModContent.Request<Texture2D>("Redemption/Textures/FadeTelegraph").Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, 64, 128), Color.DarkRed * Projectile.Opacity, npc.rotation + (Projectile.ai[1] == -1 ? MathHelper.Pi : 0), new Vector2(0, 64), new Vector2(Projectile.localAI[0] / 60f, Projectile.width / 128f), SpriteEffects.None, 0);
                Main.EntitySpriteDraw(ModContent.Request<Texture2D>("Redemption/Textures/FadeTelegraphCap").Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, 64, 128), Color.DarkRed * Projectile.Opacity, npc.rotation + (Projectile.ai[1] == -1 ? MathHelper.Pi : 0), new Vector2(0, 64), new Vector2(Projectile.width / 128f, Projectile.width / 128f), SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.BeginDefault();
            }
            return false;
        }
    }
}