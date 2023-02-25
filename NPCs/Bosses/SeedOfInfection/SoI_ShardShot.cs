using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Globals;
using Redemption.Buffs.Debuffs;

namespace Redemption.NPCs.Bosses.SeedOfInfection
{
    public class SoI_ShardShot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Shard Shot");
            Main.projFrames[Projectile.type] = 3;
            ElementID.ProjPoison[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 160;
        }
        public override void OnHitPlayer(Player target, int damage, bool crit) => target.AddBuff(ModContent.BuffType<BileDebuff>(), 120);
        public override void AI()
        {
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;

            int dustID = Dust.NewDust(new Vector2(Projectile.Center.X - 1, Projectile.Center.Y - 1), 2, 2, DustID.GreenFairy);
            Main.dust[dustID].velocity *= 0f;
            Main.dust[dustID].noGravity = true;

            if (Projectile.ai[0] == 1)
                Projectile.velocity.Y += 0.2f;
        }

        public override void Kill(int timeLeft)
        {
            int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenFairy, Scale: 1.5f);
            Main.dust[dustIndex].velocity *= 2f;
        }
    }
}
