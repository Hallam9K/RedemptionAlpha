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
    public class TearOfPainBall : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tear of Pain");
            ElementID.ProjPoison[Type] = true;
        }
        public override void SetDefaults()
        {
            Projectile.width = 34;
            Projectile.height = 34;
            Projectile.penetrate = 2;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.tileCollide = true;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 200;
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo info) => target.AddBuff(ModContent.BuffType<BileDebuff>(), 300);
        public override Color? GetAlpha(Color lightColor) => Color.White * Projectile.Opacity;
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, 0, Projectile.Opacity * 0.8f, 0);
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath1 with { Volume = .3f }, Projectile.position);
            for (int i = 0; i < 30; i++)
            {
                int dustIndex = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<SludgeDust>(), Scale: 2);
                Main.dust[dustIndex].velocity *= 2f;
            }
        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
            {
                for (int i = 0; i < 2; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(-Projectile.velocity.X + Main.rand.Next(-2, 2), -Projectile.velocity.Y + Main.rand.Next(-2, 2)), ModContent.ProjectileType<TearOfPain>(), Projectile.damage / 2, Projectile.knockBack, Projectile.owner, 0, 1);

                Projectile.Kill();
            }
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                    Projectile.velocity.X = -oldVelocity.X;
                if (Projectile.velocity.Y != oldVelocity.Y)
                    Projectile.velocity.Y = -oldVelocity.Y;
                Projectile.velocity *= 0.75f;
                SoundEngine.PlaySound(SoundID.NPCDeath1, Projectile.position);
            }
            return false;
        }
    }
}