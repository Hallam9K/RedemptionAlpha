using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Redemption.Items.Weapons.PreHM.Ranged
{
    public class GoldChickenEgg_Proj : ModProjectile
    {
        public override string Texture => "Redemption/Items/Weapons/PreHM/Ranged/GoldChickenEgg";
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gold Chicken Egg");
        }
        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 8;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 1200;
        }

        public override void AI()
        {
            Projectile.rotation += Projectile.velocity.X / 30;
            Projectile.velocity.Y += 0.3f;

            if (!Main.rand.NextBool(10))
                return;

            int sparkle = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.GoldCoin, 0, 0, 20);
            Main.dust[sparkle].velocity *= 0;
            Main.dust[sparkle].noGravity = true;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath11 with { Volume = .5f }, Projectile.position);
            for (int i = 0; i < 6; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.MothronEgg, Projectile.velocity.X * 0.5f,
                    Projectile.velocity.Y * 0.5f, Scale: 2);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.velocity.X = -Projectile.velocity.X;
            Projectile.velocity.Y = -Projectile.velocity.Y;
            Projectile.velocity *= 0.9f;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            Projectile.velocity.X = -Projectile.velocity.X;
            Projectile.velocity.Y = -Projectile.velocity.Y;
            Projectile.velocity *= 0.9f;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Collision.HitTiles(Projectile.position, oldVelocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Tink, Projectile.position);
            Projectile.penetrate--;
            if (Projectile.penetrate <= 0)
                Projectile.Kill();
            else
            {
                if (Projectile.velocity.X != oldVelocity.X)
                {
                    Projectile.velocity.X = -oldVelocity.X;
                }
                if (Projectile.velocity.Y != oldVelocity.Y)
                {
                    Projectile.velocity.Y = -oldVelocity.Y;
                }
                Projectile.velocity *= 0.9f;
            }
            return false;
        }
    }
}