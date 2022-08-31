using Terraria.Audio;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Projectiles.Ranged
{
    public class GhastlyRecurve_Proj : ModProjectile
    {
        public override string Texture => "Terraria/Images/NPC_" + NPCID.DungeonSpirit;
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Ghastly Spirit");
            Main.projFrames[Projectile.type] = 3;
        }
        public override void SetDefaults()
        {
            Projectile.width = 40;
            Projectile.height = 40;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 520;
            Projectile.penetrate = -1;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];
            if (++Projectile.frameCounter >= 5)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 3)
                    Projectile.frame = 0;
            }
            int d2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
            Main.dust[d2].noGravity = true;
            Vector2 vector = new Vector2(Projectile.ai[0], Projectile.ai[1]) - Projectile.Center;
            if (vector.Length() < Projectile.velocity.Length())
            {
                Projectile.velocity *= 0f;
                Projectile.rotation = 0;
                Projectile.localAI[0] = 1;
            }
            else if (Projectile.localAI[0] == 0)
            {
                vector.Normalize();
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, vector * 11.2f, 0.1f);
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile proj = Main.projectile[i];
                if (!proj.active || !proj.arrow || !proj.friendly || !Projectile.Hitbox.Intersects(proj.Hitbox))
                    continue;

                SoundEngine.PlaySound(SoundID.Zombie53 with { Volume = 0.6f }, Projectile.Center);
                for (int j = 0; j < 10; j++)
                {
                    int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                    Main.dust[d].velocity *= 3f;
                }
                proj.active = false;
                Projectile.NewProjectile(proj.GetSource_FromAI(), proj.position, proj.velocity, ModContent.ProjectileType<SpiritArrow_Proj>(), proj.damage, proj.knockBack, player.whoAmI);
            }
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * Projectile.Opacity;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 10; i++)
            {
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.DungeonSpirit, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, Scale: 2);
                Main.dust[d].noGravity = true;
            }
            SoundEngine.PlaySound(SoundID.NPCDeath39 with { Volume = 0.4f }, Projectile.position);
        }
    }
}