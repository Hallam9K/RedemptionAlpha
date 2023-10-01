using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Redemption.Globals;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3_JojoFist : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("King Slayer III");
            Main.projFrames[Projectile.type] = 7;
        }
        public override void SetDefaults()
        {
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.timeLeft = 120;
        }

        public Vector2 vector;
        public float offset;
        public override void AI()
        {
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 7)
                {
                    Projectile.frame = 0;
                    Projectile.Kill();
                }
            }
            if (Projectile.frame > 3)
                Projectile.hostile = false;

            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active || (npc.type != ModContent.NPCType<KS3>() && npc.type != ModContent.NPCType<KS3_Clone>()))
                Projectile.Kill();

            Vector2 HitPos = new((28 * npc.spriteDirection) + Main.rand.Next(-8, 8), -18 + Main.rand.Next(-18, 18));
            switch (Projectile.localAI[1])
            {
                case 0:
                    vector = HitPos;
                    Projectile.localAI[1] = 1;
                    break;
                case 1:
                    Projectile.Center = new Vector2(npc.Center.X + vector.X + offset, npc.Center.Y + vector.Y);
                    if (Projectile.frame > 1)
                        offset += 15 * Projectile.RightOfDir(npc);
                    break;
            }
            Projectile.velocity = Vector2.Zero;
            Projectile.rotation = (float)Math.PI / 2;
            if (npc.RightOf(Projectile))
                Projectile.rotation = (float)-Math.PI / 2;
            else
                Projectile.rotation = (float)Math.PI / 2;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;
            int height = texture.Height / 7;
            int y = height * Projectile.frame;
            Rectangle rect = new(0, y, texture.Width, height);
            Vector2 drawOrigin = new(texture.Width / 2, Projectile.height / 2);
            SpriteEffects effects = Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;

            Main.spriteBatch.End();
            Main.spriteBatch.BeginAdditive();

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle?(rect), Projectile.GetAlpha(Color.White), Projectile.rotation, drawOrigin, Projectile.scale, effects, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.BeginDefault();
            return false;
        }
    }
}