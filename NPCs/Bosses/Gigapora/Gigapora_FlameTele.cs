using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Redemption.Globals;

namespace Redemption.NPCs.Bosses.Gigapora
{
    public class Gigapora_FlameTele : Gigapora_BoltTele
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Flames");
        }
        public override void SetDefaults() => base.SetDefaults();
        public override bool PreAI()
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
                return false;
            }
            Projectile.localAI[0] += 5;
            Projectile.alpha -= 2;
            if (Projectile.alpha <= 0)
                Projectile.localAI[1] = 1;
            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (Projectile.ai[0] > -1)
            {
                Main.spriteBatch.End();
                Main.spriteBatch.BeginAdditive();

                Main.EntitySpriteDraw(ModContent.Request<Texture2D>("Redemption/Textures/FadeTelegraph").Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, 64, 128), Color.OrangeRed * Projectile.Opacity, npc.rotation + (Projectile.ai[1] == -1 ? MathHelper.Pi : 0), new Vector2(0, 64), new Vector2(Projectile.localAI[0] / 60f, Projectile.width / 128f), SpriteEffects.None, 0);
                Main.EntitySpriteDraw(ModContent.Request<Texture2D>("Redemption/Textures/FadeTelegraphCap").Value, Projectile.Center - Main.screenPosition, new Rectangle(0, 0, 64, 128), Color.OrangeRed * Projectile.Opacity, npc.rotation + (Projectile.ai[1] == -1 ? MathHelper.Pi : 0), new Vector2(0, 64), new Vector2(Projectile.width / 128f, Projectile.width / 128f), SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.BeginDefault();
            }
            return false;
        }
    }
}