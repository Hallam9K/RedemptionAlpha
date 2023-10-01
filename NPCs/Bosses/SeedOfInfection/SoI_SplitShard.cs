using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Redemption.Globals;
using Terraria.Audio;

namespace Redemption.NPCs.Bosses.SeedOfInfection
{
    public class SoI_SplitShard : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Splitting Shard");
            Main.projFrames[Projectile.type] = 7;
            ElementID.ProjPoison[Type] = true;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
        }
        public override void AI()
        {
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 7)
                    Projectile.frame = 2;
            }
            Projectile.velocity *= .96f;
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White * Projectile.Opacity;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            for (int i = 0; i < 5; ++i)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenFairy, Scale: 1.5f);
                Main.dust[dustIndex].velocity *= 2f;
            }
            if (Main.myPlayer == Projectile.owner)
            {
                int spread = Main.expertMode ? 8 : 16;
                for (int i = 0; i < (Main.expertMode ? 5 : 3); i++)
                {
                    int rot = spread * i;
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, RedeHelper.PolarVector(10, Projectile.velocity.ToRotation() + MathHelper.ToRadians(rot - spread)), ModContent.ProjectileType<SoI_ShardShot>(), Projectile.damage, Projectile.knockBack, Main.myPlayer);
                }
            }
        }
    }
}
