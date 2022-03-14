using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.Base;

namespace Redemption.NPCs.Bosses.SeedOfInfection
{
    public class StrangePortal : ModProjectile
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
        }

        private bool rotSwitch;
        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            ScreenPlayer modPlayer = player.RedemptionScreen();
            if (Projectile.localAI[0]++ == 0)
            {
                if (!Main.dedServ)
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue("A portal to another world has opened!", 120, 30, 0.8f, null, 1f, Color.Green);

                modPlayer.Rumble(180, 3);
            }

            if (Projectile.localAI[0] % 10 == 0 && Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<StrangePortal2>(), 0, 0, Projectile.owner, rotSwitch ? 0 : 1);
                rotSwitch = !rotSwitch;
            }
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 30; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.GreenFairy, 0f, 0f, 100, default, 3.5f);
                Main.dust[dustIndex].velocity *= 3f;
            }
        }
    }
    public class StrangePortal2 : ModProjectile
    {
        public override string Texture => "Redemption/NPCs/Bosses/SeedOfInfection/StrangePortal";

        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Strange Portal");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 80;
            Projectile.alpha = 150;
        }
        public override void AI()
        {
            Projectile.scale++;
            Projectile.alpha += 2;
            if (Projectile.alpha >= 255)
                Projectile.Kill();

            if (++Projectile.frameCounter >= 15)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }

            if (Projectile.ai[0] == 0)
                Projectile.rotation += 0.09f;
            else
                Projectile.rotation -= 0.09f;
        }
    }
}