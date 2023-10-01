using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Localization;
using Redemption.BaseExtension;
using ParticleLibrary;
using Redemption.Particles;

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
                    RedeSystem.Instance.DialogueUIElement.DisplayDialogue(Language.GetTextValue("Mods.Redemption.Cutscene.SoI"), 120, 30, 0.8f, null, 1f, Color.Green);

                modPlayer.Rumble(180, 3);
            }

            if (Projectile.localAI[0] % 10 == 0 && Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<StrangePortal2>(), 0, 0, Projectile.owner, rotSwitch ? 0 : 1);
                rotSwitch = !rotSwitch;
            }
        }
        public override void OnKill(int timeLeft)
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
        public override string Texture => "Redemption/Textures/PortalTex";

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Strange Portal");
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 2400;
        }
        public override void SetDefaults()
        {
            Projectile.width = 188;
            Projectile.height = 188;
            Projectile.penetrate = -1;
            Projectile.hostile = false;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 80;
            Projectile.alpha = 50;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Green * Projectile.Opacity;
        }
        public override void AI()
        {
            Projectile.scale += 0.5f;
            Projectile.alpha += 4;
            if (Projectile.alpha >= 255)
                Projectile.Kill();

            if (Projectile.ai[1]++ % 12 == 0)
            {
                Vector2 spawnPos = new Vector2(0f, -50f).RotatedBy(MathHelper.ToRadians(Main.rand.NextFloat(360f)));

                ParticleManager.NewParticle(Projectile.Center, spawnPos.RotatedBy(Main.rand.NextFloat(-30f, 30f)), new AnglonPortal_EnergyGather(), Color.White, 1f, Projectile.Center.X, Projectile.Center.Y);
            }

            if (Projectile.ai[0] == 0)
                Projectile.rotation += 0.09f;
            else
                Projectile.rotation -= 0.09f;
        }
    }
}
