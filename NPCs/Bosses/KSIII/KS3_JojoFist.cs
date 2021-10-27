using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3_JojoFist : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("King Slayer III");
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
            Projectile.alpha = 0;
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
            if (!npc.active || npc.type != ModContent.NPCType<KS3>())
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
                    {
                        if (npc.Center.X > Projectile.Center.X)
                            offset -= 15;
                        else
                            offset += 15;
                    }
                    break;
            }
            Projectile.velocity = Vector2.Zero;
            Projectile.rotation = (float)Math.PI / 2;
            if (npc.Center.X > Projectile.Center.X)
                Projectile.rotation = (float)-Math.PI / 2;
            else
                Projectile.rotation = (float)Math.PI / 2;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
    }
}