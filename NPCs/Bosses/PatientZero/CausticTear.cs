using Microsoft.Xna.Framework;
using Redemption.Buffs.Debuffs;
using Redemption.Dusts;
using Redemption.Globals;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.NPCs.Bosses.PatientZero
{
    public class CausticTear : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Caustic Tear");
            Main.projFrames[Projectile.type] = 4;
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.penetrate = 1;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 200;
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
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<BileDebuff>(), 180);
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1 with { Volume = .3f }, Projectile.position);
            for (int i = 0; i < 10; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SludgeDust>(), Scale: 2);
                Main.dust[dustIndex].velocity *= 2f;
            }
        }
    }
    public class InfectiousBeat : CausticTear
    {
        public override string Texture => "Redemption/NPCs/Bosses/PatientZero/CausticTear";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Infectious Beat");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults() => base.SetDefaults();

        public override void PostAI()
        {
            if (Projectile.localAI[0] == 1 && Main.myPlayer == Projectile.owner)
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, Projectile.velocity, ModContent.ProjectileType<InfectiousBeat_Tele>(), 0, 0, Main.myPlayer);

            Projectile.velocity.Y += 0.2f;
        }
    }
    public class InfectiousBeat_Tele : CausticTear
    {
        public override string Texture => Redemption.EMPTY_TEXTURE;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Infectious Beat");
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
            Projectile.velocity.Y += 0.2f;
        }
    }
}