/*using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace Redemption.NPCs.Bosses.Neb.Phase2
{
    public class ConstellationsStarNeb : ModProjectile
    {
        public override string Texture => "Redemption/Projectiles/Magic/ConstellationsStar";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Constellations");
            Main.projFrames[projectile.type] = 7;
        }

        public override void SetDefaults()
        {
            projectile.width = 22;
            projectile.height = 30;
            projectile.aiStyle = -1;
            projectile.friendly = true;
            projectile.hostile = false;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.timeLeft = 180;
        }

        public override void AI()
        {
            Player player = Main.player[projectile.owner];
            if (++projectile.frameCounter >= 5)
            {
                projectile.frameCounter = 0;
                if (++projectile.frame >= 7)
                {
                    projectile.frame = 2;
                }
            }
            Lighting.AddLight(projectile.Center, (255 - projectile.alpha) * 0.4f / 255f, (255 - projectile.alpha) * 0.4f / 255f, (255 - projectile.alpha) * 0.4f / 255f);
            NPC npc2 = Main.npc[(int)projectile.ai[0]];

            projectile.localAI[0]++;
            if (projectile.ai[1] == 0)
            {
                if (projectile.localAI[0] == 120 && Main.myPlayer == projectile.owner)
                {
                    Projectile.NewProjectile(new Vector2(projectile.Center.X, projectile.Center.Y), Vector2.Zero, ModContent.ProjectileType<ConstellationsStarNeb>(), projectile.damage, projectile.knockBack, projectile.owner, projectile.whoAmI);
                }
            }
            if (projectile.ai[1] > 20)
            {
                projectile.Kill();
            }
            else
            {
                if (projectile.localAI[0] == 30 && Main.myPlayer == projectile.owner)
                {
                    Projectile.NewProjectile(new Vector2(npc2.Center.X + Main.rand.Next(-500, 500), npc2.Center.Y + Main.rand.Next(-500, 500)), Vector2.Zero, ModContent.ProjectileType<ConstellationsStarNeb>(), projectile.damage, projectile.knockBack, projectile.owner, npc2.whoAmI, projectile.ai[1] + 1);
                }
            }
        }
    }
}*/