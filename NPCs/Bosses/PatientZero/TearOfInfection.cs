using Microsoft.Xna.Framework;
using Redemption.Dusts;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Redemption.BaseExtension;

namespace Redemption.NPCs.Bosses.PatientZero
{
    public class TearOfInfection : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tear of Infection");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.penetrate = 1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 200;
            Projectile.Redemption().Unparryable = true;
        }
        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;
        public override void AI()
        {
            if (++Projectile.frameCounter >= 3)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 4)
                    Projectile.frame = 0;
            }
            if (++Projectile.localAI[0] > 8)
                Projectile.tileCollide = true;

            Lighting.AddLight(Projectile.Center, 0, Projectile.Opacity * 0.8f, 0);
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            Projectile.velocity.Y += 0.1f;
        }
        public override void Kill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1 with { Volume = .3f }, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SludgeDust>(), Scale: 2);
                Main.dust[dustIndex].velocity *= 2f;
            }
        }
    }
    public class PoisonBeat : TearOfInfection
    {
        public override string Texture => "Redemption/NPCs/Bosses/PatientZero/TearOfInfection";
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Poison Beat");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
        }
        public override void PostAI()
        {
            if (Projectile.localAI[0] == 1 && Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<PoisonBeat_Tele>(), 0, 0, Main.myPlayer);

            Projectile.velocity.Y += 0.1f;
        }
    }
    public class PoisonBeat_Tele : TearOfInfection
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Poison Beat");
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.extraUpdates = 1000;
        }

        public override void PostAI()
        {
            Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.GreenTorch, Vector2.Zero);
            dust.velocity *= 0;
            dust.noGravity = true;
            Projectile.velocity.Y += 0.1f;
        }
    }
}