using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Redemption.Globals;

namespace Redemption.NPCs.Bosses.Obliterator
{
    public class OO_Fingerflash : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fingerflash");
            Main.projFrames[Projectile.type] = 9;
        }
        public override void SetDefaults()
        {
            Projectile.width = 52;
            Projectile.height = 52;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 120;
        }
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active || npc.type != ModContent.NPCType<OO>())
                Projectile.Kill();

            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (Projectile.frame == 6)
                {
                    (Main.npc[npc.whoAmI].ModNPC as OO).ArmRot[0] = MathHelper.PiOver2 + (0.7f * -npc.spriteDirection) + (npc.spriteDirection == -1 ? 0 : MathHelper.Pi);

                    Projectile.Shoot(Projectile.Center, ModContent.ProjectileType<OO_Laser>(), 150, RedeHelper.PolarVector(npc.DistanceSQ(Main.player[npc.target].Center) >= 900 * 900 ? 30 : 12, (Main.player[npc.target].Center - npc.Center).ToRotation()), true, CustomSounds.Laser1);
                }
                if (++Projectile.frame >= 9)
                    Projectile.Kill();
            }
            Projectile.rotation = MathHelper.PiOver2 * npc.spriteDirection;
            Projectile.Center = npc.Center + new Vector2(npc.spriteDirection == -1 ? -110 : 64, -6);
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
    }
}