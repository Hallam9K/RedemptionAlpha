using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.NPCs.Lab.Janitor
{
    public class JanitorMop_Proj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Mop");
        }
        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 300;
            Projectile.ignoreWater = false;
            Projectile.tileCollide = true;
        }
        public override void AI()
        {
            switch (Projectile.ai[0])
            {
                case 0:
                    Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
                    Projectile.velocity.Y += 0.02f;
                    break;
                case 1:
                    if (Projectile.velocity.X > 0)
                        Projectile.rotation += 0.5f;
                    else
                        Projectile.rotation -= 0.5f;
                    break;
            }
            if (Projectile.localAI[0]++ >= 30)
                Projectile.friendly = true;
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.ai[0] == 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WoodFurniture, 0f, 0f, 100, default, 2.0f);
                    Main.dust[dustIndex].velocity *= 2f;
                }
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X;
                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
                Projectile.velocity *= 0.95f;
                return false;
            }
            else
                return true;
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (target.type == ModContent.NPCType<JanitorBot>() && target.ai[0] != 4 && target.ai[0] != 5)
            {
                target.RedemptionGuard().GuardPoints = 0;
                target.ai[0] = 4;
                target.ai[1] = 0;
                target.ai[2] = 0;
                target.RedemptionGuard().GuardBreakCheck(target, DustID.Electric, CustomSounds.GuardBreak, 10, 1, 1000);
            }
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
            for (int i = 0; i < 8; i++)
                Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.WoodFurniture, -Projectile.velocity.X * 0.2f,
                    -Projectile.velocity.Y * 0.2f);
        }
    }
}
