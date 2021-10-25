using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.KSIII
{
    public class KS3_Surge : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Core Surge");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 100;
            Projectile.height = 100;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 60;
        }
        public override void AI()
        {
            NPC npc = Main.npc[(int)Projectile.ai[0]];
            if (!npc.active || npc.type != ModContent.NPCType<KS3>())
                Projectile.Kill();

            Projectile.Center = npc.Center;
            Projectile.scale += 0.2f;

            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }

            Projectile.alpha += 10;
            if (Projectile.alpha >= 255)
                Projectile.Kill();

            for (int k = 0; k < Main.maxPlayers; k++)
            {
                Player player = Main.player[k];
                if (!player.active || player.dead || Projectile.DistanceSQ(player.Center) >= 200 * 200)
                    continue;

                player.AddBuff(BuffID.Electrified, 180);
            }
        }
    }
    public class KS3_Surge2 : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Core Surge");
        }
        public override void SetDefaults()
        {
            Projectile.width = 80;
            Projectile.height = 80;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = false;
            Projectile.timeLeft = 20;
            Projectile.alpha = 255;
        }

        public override void AI()
        {
            for (int p = 0; p < Main.maxPlayers; p++)
            {
                Player player = Main.player[p];
                if (player.active || !player.noKnockback || !Projectile.Hitbox.Intersects(player.Hitbox))
                    continue;

                player.velocity = Projectile.velocity;
            }
        }
    }
}